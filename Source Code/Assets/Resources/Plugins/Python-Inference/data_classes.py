from torch_geometric.data import Data, Dataset, DataLoader
from torch_geometric.utils import degree
import torch
from datetime import datetime
import numpy as np

def pairwise_square_distances(x, y=None):
    '''
    Input: x is a Nxd matrix
           y is an optional Mxd matirx
    Output: dist is a NxM matrix where dist[i,j] is the square norm between x[i,:] and y[j,:]
            if y is not given then use 'y=x'.
    i.e. dist[i,j] = ||x[i,:]-y[j,:]||^2
    '''
    x_norm = (x**2).sum(1).view(-1, 1)
    if y is not None:
        y_t = torch.transpose(y, 0, 1)
        y_norm = (y**2).sum(1).view(1, -1)
    else:
        y_t = torch.transpose(x, 0, 1).double()
        y_norm = x_norm.view(1, -1)

    dist = x_norm + y_norm - 2.0 * torch.mm(x, y_t)
    # Ensure diagonal is zero if x=y
    # if y is None:
    #     dist = dist - torch.diag(dist.diag)
    return torch.clamp(dist, 0.0, np.inf).double()

class SimpleTrajData(Data):
    def __init__(self, positions, edges_per_point, model, alpha, noise, clip_trajs, compute_convex_hull, plot=False, scale_types=["step_std"], include_moments=False):

        positions = positions.double()
        positions = positions - torch.mean(positions,dim=0)

        while torch.max(torch.abs(positions)) > 1e7:
            positions = positions / 1e7

        while torch.max(torch.abs(positions)) < 1e-7:
            positions = positions * 1e7

        # Centered and scaled positions

        jumps = positions[1:] - positions[:-1]

        if clip_trajs:
            dr = torch.sqrt(torch.sum(jumps**2,dim=1))
            median_dr = torch.median(dr)
            dr[dr == 0.] = 1. # Comme ça, pour eux u = 0.
            u = jumps / dr.view(-1,1)
            clipped_dr = torch.clamp(dr, 0., 10*median_dr)
            old_positions_shape = positions.shape
            old_jumps = jumps.clone()
            positions[1:] = positions[0] + torch.cumsum(clipped_dr.view(-1,1)*u,dim=0)
            assert positions.shape == old_positions_shape
            jumps = positions[1:] - positions[:-1]
            #print(jumps[:10], old_jumps[:10])

        N = positions.shape[0] - 1
        dim = positions.shape[1]

        #print("dim = %d" % dim)

        t = torch.arange(N) * 1e-2

        dr = torch.sqrt(torch.sum(jumps**2,dim=1))
        start = positions[0]
        positions = positions[:-1]

        with torch.no_grad():
            # Distance matrice
            alpha_fit, alpha_curve, Dist_matrix = self.compute_alpha_from_positions(positions, true_alpha=alpha, plot=plot)

        #jumps = jumps[1:]
        #dr = dr[1:]

        # cum_distance[i] distance parcourue du saut 0 au saut i. Le saut 0 est celui qui arrive au point 0
        cum_distance = torch.cumsum(dr,dim=0)
        cum_distance_2 = torch.pow(torch.cumsum(dr**2,dim=0),1./2)
        cum_distance_4 = torch.pow(torch.cumsum(dr**4,dim=0),1./4)

        N_vector = torch.ones(dr.shape[0])*N *1e-2

        # Compute Scales
        if compute_convex_hull:
            if positions.shape[1] > 1:
                try:
                    scale_hull = torch.pow(torch.Tensor([ConvexHull(positions).volume]).double(), torch.Tensor([1./positions.shape[1]]))
                except:
                    scale_hull = 1e-7
            else:
                scale_hull = torch.max(positions) - torch.min(positions)
        scale_hull = 1.
        scale_mean = torch.mean(dr)
        scale_sum = torch.sum(dr)
        scale_std = torch.std(dr)

        x_scaled = []

        norm_fn = lambda x: torch.log(x)
        #norm_fn = lambda x: x

        scales = []
        if "step_std" in scale_types:
            scales.append(scale_std)
        if "step_mean" in scale_types:
            scales.append(scale_mean)
        if "step_sum" in scale_types:
            scales.append(scale_sum)
        if "hull" in scale_types:
            scales.append(scale_hull)

        for scale in scales:
            if scale < 1e-5:
                scale = 1e-5
            if scale > 1e7:
                scale = 1e7
            assert scale > 0.

            x_scaled += [torch.cat([jumps / scale,
                             norm_fn(1e-7 + cum_distance_2.view(-1,1) / scale),
                             norm_fn(1e-7 + cum_distance_4.view(-1,1) / scale),
                             norm_fn(1e-7 + cum_distance.view(-1,1).double() / scale)],dim=1).clone()]
        x = torch.cat(x_scaled + [torch.cat([norm_fn(1+t).view(-1,1).double(),
                                             N_vector.view(-1,1).double()],dim=1)],dim=1)



        edges_start = torch.cat([torch.arange(N)]*edges_per_point)
        # Eviter les arrêtes vers des voisins directs
        closest = 1
        edges_end = edges_start + np.power(10,np.random.uniform(np.log10(closest), np.log10(N-(closest)))).astype(int)
        #edges_end = edges_start + torch.randint(low=2,high=N-1,size=(N*edges_per_point,))
        edges_end = edges_end % N
        edges = torch.stack([edges_start, edges_end],dim=0)
        dt = (edges_end - edges_start)*1e-2

        delta_pos = positions[edges_end] - positions[edges_start]
        distance_2 = torch.sum(delta_pos**2,dim=1)
        distance = torch.sqrt(distance_2)

        edges_min = torch.where(edges_start < edges_end, edges_start, edges_end)
        edges_max = torch.where(edges_start < edges_end, edges_end, edges_start)

        cum_distance_edges = cum_distance[edges_max] - cum_distance[edges_min]
        cum_distance_edges_2 = cum_distance_2[edges_max] - cum_distance_2[edges_min]
        cum_distance_edges_4 = cum_distance_4[edges_max] - cum_distance_4[edges_min]
        scaled_e = []
        ## Global scales
        for scale in scales:
            if scale <1e-5:
                scale = 1e-5
            if scale > 1e7:
                scale = 1e7
            assert scale > 0.
            scaled_e += [torch.cat([delta_pos / scale,
                                   norm_fn(1e-7 + cum_distance_edges.view(-1,1) / scale),
                                   norm_fn(1e-7 + cum_distance_edges_2.view(-1,1) / scale),
                                   norm_fn(1e-7 + cum_distance_edges_4.view(-1,1) / scale),
                                   norm_fn(1e-7 + distance.view(-1,1) / scale)],dim=1)]

        N_vector = torch.ones(dt.shape[0]).double()*N*1e-2
        edge_attr = torch.cat(scaled_e + [torch.cat([N_vector.view(-1,1),
                                                     dt.view(-1,1).double(),
                                                     norm_fn(torch.abs(dt).view(-1,1).double())],dim=1)], dim = 1)


        super(SimpleTrajData,self).__init__(pos = positions.float(),
                                  x = x.float(),
                                  edge_index = edges.long(),
                                  edge_attr = edge_attr.float(),
                                  model = torch.Tensor([model]).long(),
                                  noise = torch.Tensor([noise]).view(1,1).float(),
                                  alpha = torch.Tensor([alpha]).view(1,1).float(),
                                  alpha_fit = torch.tensor([alpha_fit]).view(-1,1).float(),
                                  n_points = torch.Tensor([x.shape[0] + 2.]).view(1,1),
                                  scale = torch.Tensor([scale_sum, scale_std, scale_hull,scale_mean]).float())
        self.check_numeric()

    def compute_alpha_from_positions(self, pos, true_alpha, best_N=None, plot=False):

        start_time = datetime.now()

        D2 = pairwise_square_distances(pos)
        #print(D2)
        N = pos.shape[0]
        start = 2
        end = min(10,N-2)
        if abs(start - end) <= 2:
            start = 1
            end += 1
        #end = 10
        log_MSD = torch.DoubleTensor([torch.log(torch.mean(torch.diagonal(D2,offset=i))) for i in range(start,end)])

        log_time = torch.log(torch.arange(end-start).double()+start)
        if plot:
            plt.figure()
            plt.plot(pos[:,0])
            plt.figure()
            plt.plot(log_time, log_MSD)
        # Fit des moindres carrés de la log_MSD par le log_temps

        N = log_time.shape[0]
        #w = torch.arange(N) + 1
        #w = torch.flip(w,dims=[0])
        w = torch.ones(N)
        N = torch.sum(w)
        alpha = torch.sum(log_MSD*log_time*w) - (1./N)*torch.sum(log_time*w)*torch.sum(log_MSD*w)
        alpha = alpha / (torch.sum((log_time**2)*w) - (1./N)*(torch.sum(w*log_time)**2))
        alpha = torch.tanh(alpha-1) + 1
        #alpha = torch.clamp(alpha,0.01,2.)

        intercept = (1./N)*(torch.sum(log_MSD*w)-alpha*torch.sum(log_time*w))
        end_time = datetime.now()
        if plot:
            plt.plot(log_time, [intercept + alpha*t for t in log_time])
            plt.title("alpha = %.2f | fit = %.2f" % (true_alpha,alpha))
        #print("Computed MSD fit in %.2f " % (end_time-start_time).total_seconds() )
        return alpha, torch.stack([log_MSD,log_time],dim=1), torch.sqrt(D2)

    def check_numeric(self):
        try:
            assert torch.isnan(self.x).sum() == 0
            assert torch.isnan(self.edge_attr).sum() == 0
            assert torch.isinf(self.x).sum() == 0
            assert torch.isinf(self.edge_attr).sum() == 0
        except:
            print("Scale")
            print(self.scale)
            print("X")
            print(torch.sum(1.*(torch.isnan(self.x) + torch.isinf(self.x)),dim=0))
            print("edge_attr")
            print(torch.sum(1.*(torch.isnan(self.edge_attr) + torch.isinf(self.edge_attr)),dim=0))
            raise

def traj_to_array(positions, dim):
    N = len(positions) // dim
    return torch.Tensor(positions).double().reshape(N, dim)


class TrajListDataset(Dataset):

    def __init__(self, trajectories, alphas, scale_types = ["step_std","step_mean","step_sum"]):
        super(TrajListDataset,self).__init__()
        self.trajectories = trajectories
        self.edges_per_point = 5
        self.scale_types = scale_types
        self.alphas = alphas
        self.plot = False

    def __len__(self):
        return len(self.trajectories)

    def get(self,idx):
        data = SimpleTrajData(positions=self.trajectories[idx],
                      edges_per_point = self.edges_per_point,
                      model=0,
                      noise=1.,
                      clip_trajs = True,
                      scale_types = self.scale_types,
                      compute_convex_hull=False,
                      alpha=self.alphas[idx],
                      plot=self.plot,
                      include_moments=True)

        return data
