from torch_geometric.utils import degree
from torch_geometric.nn.conv import MessagePassing
from torch_geometric.nn import GlobalAttention, global_mean_pool
from sklearn.neighbors import NearestNeighbors
from sklearn.preprocessing import QuantileTransformer
import torch_geometric.transforms as T
from torch.optim.lr_scheduler import ReduceLROnPlateau

import andi
import pandas as pd
from andi.utils_andi import normalize
import torch
import torch.nn as nn
from tqdm import tqdm

import numpy as np
import matplotlib.pyplot as plt

def MLP(channels,
        activation="leaky",
        last_activation="identity",
        bias=True,
        dropout= 0.):

    if last_activation == "identity":
        last_activation = nn.Identity()

    if activation == "leaky":
        activations = [torch.nn.LeakyReLU(0.2) for i in range(1,len(channels)-1)] + [last_activation]
    else:
        activations = [torch.nn.ReLU() for i in range(1,len(channels)-1)] + [last_activation]
    return nn.Sequential(
        *[
            nn.Sequential(
                nn.Dropout(dropout),
                nn.Linear(channels[i - 1], channels[i], bias=bias),
                nn.BatchNorm1d(channels[i]),
                activations[i-1],
            )
            for i in range(1, len(channels))
        ]
    )

class JumpsConv(MessagePassing):
    def __init__(self,
                 out_channels,
                 x_dim,
                 edge_attr_dim,
                 dropout=.0,
                 aggr="mean",
                 moments = [1],
                 **kwargs):
        super(JumpsConv, self).__init__(aggr=aggr, **kwargs) # , flow="target_to_source"
        self.out_channels = out_channels
        self.p = dropout
        self.moments = moments
        M = len(moments)
        self.bn_x = nn.BatchNorm1d(2*x_dim)
        self.bn_e = nn.BatchNorm1d(2*edge_attr_dim)
        self.moment_net_x = MLP([2*x_dim,2*x_dim,x_dim])#,last_activation = nn.Tanh())
        self.moment_net_e = MLP([2*edge_attr_dim,2*edge_attr_dim,edge_attr_dim])#,last_activation = nn.Tanh())

        self.g = MLP([edge_attr_dim + 2*x_dim,128,64,32,out_channels],
                     dropout=dropout)#,last_activation = nn.Tanh()))

        if M > 1:
            self.f = nn.Sequential(nn.BatchNorm1d(M*out_channels),
                        MLP([M*out_channels,128,64,out_channels], dropout=dropout))#last_activation=nn.Tanh()))
        else:
            self.f = MLP([M*out_channels,128,64,out_channels], dropout=dropout)

    def forward(self, x, edge_index, edge_attr):

        x = torch.cat((x,x**2),dim=1)
        x = self.bn_x(x)
        x = self.moment_net_x(x)

        edge_attr = torch.cat((edge_attr, edge_attr**2),dim=1)
        edge_attr = self.bn_e(edge_attr)
        edge_attr = self.moment_net_e(edge_attr)

        neighbors_message = self.propagate(x=x, edge_index=edge_index, edge_attr=edge_attr)
        neighbors_message = torch.cat([torch.pow(neighbors_message,m) for m in self.moments],dim=1)
        return self.f(neighbors_message)

    def message(self, edge_attr, x_j, x_i):
        # X_j is the neighbor node
        # ADD X_J to the message
        return self.g(torch.cat([edge_attr, x_j, x_i], dim=1))


class TrajsEncoder(nn.Module):
    def __init__(self, n_c = 64, latent_dim=128, p = 0.,dim=1, x_dim=1, e_dim=1):
        super(TrajsEncoder,self).__init__()
        moments = [1,2,4]
        M = len(moments)

        self.jumpsConv1 = JumpsConv(out_channels=n_c,
                                   x_dim=x_dim,
                                   edge_attr_dim=e_dim, dropout=p,
                                    aggr="mean", moments = moments)
        self.jumpsConv2 = JumpsConv(out_channels=n_c,
                                   x_dim=n_c, dropout=p,
                                   edge_attr_dim=e_dim, aggr="max")

        self.jumpsConv_final = JumpsConv(out_channels=n_c,
                                   x_dim=2*n_c, dropout=p,
                                   edge_attr_dim=e_dim, aggr="mean", moments = moments)

        gate_nn = MLP([3*n_c,n_c,n_c // 2, 1], dropout=p)
        self.pooling = GlobalAttention(gate_nn=gate_nn)
        #self.pooling = global_mean_pool

        self.mlp = MLP([3*n_c,2*n_c,2*latent_dim,latent_dim-1], dropout=p) # used to be tanh for last_activation

    def forward(self, data):

        data.x = data.x.cuda()
        data.edge_index = data.edge_index.cuda()
        data.edge_attr = data.edge_attr.cuda()
        data.batch = data.batch.cuda()

        x_1 = self.jumpsConv1(x=data.x,
                           edge_index=data.edge_index,
                           edge_attr=data.edge_attr)
        #print(x_1.shape)
        x_2 = self.jumpsConv2(x=x_1,
                           edge_index=data.edge_index,
                           edge_attr=data.edge_attr)
        #print(x_2.shape)
        x = torch.cat([x_1,x_2],dim=1)

        x = self.jumpsConv_final(x=x,
                                 edge_index = data.edge_index,
                                 edge_attr=data.edge_attr)
        x = torch.cat((x, x_1, x_2), dim=1)
        #print(x.shape)
        x = self.pooling(x=x,batch=data.batch)
        x = self.mlp(x)

        x = torch.cat([x,data.alpha_fit.cuda()],dim=1)
        return x

class Predictor(nn.Module):
    """
    The only interest of thos class is the '+1' in the forward function, because alpha is not centered in 0 but in 1
    """
    def __init__(self, p,input_dim=128):
        super(Predictor,self).__init__()
        self.bn_alpha_fit = nn.BatchNorm1d(1)
        self.mlp = nn.Sequential(MLP([input_dim,128,128,64,16,1],dropout=p))
    def forward(self,x):
        x[:,-1:] = self.bn_alpha_fit(x[:,-1:])
        residual = self.mlp(x) # Last column of x is the alpha fit by MSD
        return 1. + torch.nn.Tanh()(residual - 1 + x[:,-1:])

def smooth_point_cloud(x,y):
    N = min(20,x.shape[0] // 300)
    bins = np.quantile(x,np.linspace(0.,1.,N+1))
    s = np.zeros(len(bins)-1)
    for i in range(N):
        s[i] = np.mean(y[(x >= bins[i]) & (x < bins[i+1])])
    return 0.5*(bins[:-1] + bins[1:]), s

class TrajsNet(nn.Module):
    def __init__(self, n_classes, n_c = 64, p = 0., lr = 1e-4,
                 latent_dim=128, save_path = None,
                 dim=1, x_dim=1, e_dim=1):
        super(TrajsNet,self).__init__()

        self.n_c = n_c
        self.latent_dim = latent_dim

        ## NN Components

        self.encoder = TrajsEncoder(n_c = n_c, p = p, latent_dim=latent_dim,dim=dim, x_dim=x_dim, e_dim=e_dim)

        #self.classifier = nn.Sequential(MLP([latent_dim,128,128,64,32],
        #                                    dropout=p,
        #                                    last_activation=nn.LeakyReLU(0.2)),
        #                                 nn.Linear(32,n_classes))

        self.alpha_predictor = Predictor(p,input_dim=latent_dim)

        ## Optimizer

        self.optimizer = torch.optim.Adam(self.parameters(), lr=lr,
                                 amsgrad=True)
        self.scheduler = ReduceLROnPlateau(self.optimizer,
                                           'min',
                                           patience=6,
                                           factor=0.2,
                                           cooldown=3,
                                           min_lr=1e-5,
                                           verbose=True)

        self.save_path = save_path
        self.cuda()

    def L2_loss(self, out, target):
        l_per_sample = nn.MSELoss(reduction="none")(out, target)
        return torch.mean(l_per_sample)

    def forward(self, data):

        target_alpha = data.alpha.cuda()
        h = self.encoder(data)
        out = {}
        out["latent"] = h
        out["alpha"] = self.alpha_predictor(h)
        loss = self.L2_loss(out["alpha"], target_alpha)
        return loss, out

    def run_epoch(self, dl,store_embeddings=False,store_inputs=False):

        embeddings = None
        grad_fig = None
        print_every = 50
        N = len(dl.dataset)
        if store_embeddings:
            embeddings = torch.zeros((N,self.latent_dim))
        alpha = torch.zeros((N,5))
        losses = []

        activations = {}
        inputs = {}

        def get_store_activations(name):
            if name not in activations:
                    activations[name] = []
            def store_activations(layer, in_, out_):
                activations[name].append(out_.detach().cpu().numpy())
            return store_activations

        def store_inputs(layer, in_, out_):
            if layer not in inputs:
                inputs[layer] = []
            # input is a tuple
            inputs[layer].append(in_[0].detach().cpu().numpy())

        with torch.set_grad_enabled(self.training):
            i = 0
            x_counter = 0
            for data in tqdm(dl):
                B = data.alpha.shape[0]
                if self.training:
                    self.optimizer.zero_grad()

                loss, out = self(data)
                losses.append(loss.item())
                if self.training:
                    loss.backward()
                    nn.utils.clip_grad_value_(self.parameters(),1.)
                    self.optimizer.step()

                if store_embeddings:
                    embeddings[i:(i+data.alpha.shape[0])] = out["latent"]

                alpha[i:(i+B),0] = out["alpha"].view(-1)
                alpha[i:(i+B),1] = data.alpha.view(-1)
                alpha[i:(i+B),2] = data.n_points.view(-1)
                alpha[i:(i+B),3] = data.model.view(-1)
                alpha[i:(i+B),4] = data.noise.view(-1)
                loss_alpha = np.mean(losses[-print_every:])

                i += B

                if i // B % print_every == 0:
                    print("%d \tMSE : %.2f" % (i//B, loss_alpha))
            results_fig = None

        return losses, alpha, embeddings, grad_fig, results_fig, activations, inputs

    def plot_results(self, alpha):

        models = torch.unique(alpha[:,3])
        n_models = len(models)

        fig = plt.figure(figsize=(10,10),dpi=100)
        ax1 = fig.add_subplot(211)
        ax1.set_xlabel("Trajectory length")
        ax1.set_ylabel("Mean absolute error")
        ax1.set_title("Prediction of exponent")
        for m in models:
            x, y = smooth_point_cloud(alpha[alpha[:,3] == m,2].detach().cpu().numpy(),
                                      torch.abs(alpha[alpha[:,3] == m,0]-alpha[alpha[:,3] == m,1]).detach().cpu().numpy())
            try:
                l = andi.andi_datasets().avail_models_name[int(m)]
            except:
                l = "all models"
            ax1.plot(x,y,label=l)
        ax1.legend()

        ax2 = fig.add_subplot(212)
        ax2.set_xlabel("Noise ratio")
        ax2.set_ylabel("Mean absolute error")
        x, y = smooth_point_cloud(alpha[:,4].detach().cpu().numpy(),
                                  torch.abs(alpha[:,0]-alpha[:,1]).detach().cpu().numpy())
        ax2.plot(x,y)
        plt.tight_layout()
        return fig


    def plot_grad_flow(self):
        '''Plots the gradients flowing through different layers in the net during training.
        Can be used for checking for possible gradient vanishing / exploding problems.

        Usage: Plug this function in Trainer class after loss.backwards() as
        "plot_grad_flow(self.model.named_parameters())" to visualize the gradient flow'''
        named_parameters = self.named_parameters()
        ave_grads = []
        max_grads= []
        layers = []
        for n, p in named_parameters:
            if(p.requires_grad) and ("bias" not in n):
                layers.append(n)
                ave_grads.append(p.grad.abs().mean())
                max_grads.append(p.grad.abs().max())
        plt.bar(np.arange(len(max_grads)), max_grads, alpha=0.1, lw=1, color="c")
        plt.bar(np.arange(len(max_grads)), ave_grads, alpha=0.1, lw=1, color="b")
        plt.hlines(0, 0, len(ave_grads)+1, lw=2, color="k" )
        plt.xticks(range(0,len(ave_grads), 1), layers, rotation="vertical")
        plt.xlim(left=0, right=len(ave_grads))
        #plt.ylim(bottom = -0.001, top=0.02) # zoom in on the lower gradient regions
        plt.xlabel("Layers")
        plt.ylabel("average gradient")
        plt.title("Gradient flow")
        plt.grid(True)
        plt.legend([Line2D([0], [0], color="c", lw=4),
                    Line2D([0], [0], color="b", lw=4),
                    Line2D([0], [0], color="k", lw=4)], ['max-gradient', 'mean-gradient', 'zero-gradient'],
                   loc="lower center")
