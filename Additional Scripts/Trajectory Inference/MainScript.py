from torch_geometric.utils import degree
from torch_geometric.nn.conv import MessagePassing
from torch_geometric.nn import GlobalAttention, global_mean_pool
from sklearn.neighbors import NearestNeighbors
from sklearn.preprocessing import QuantileTransformer
import torch_geometric.transforms as T

import andi
import pandas as pd
from andi.utils_andi import normalize
import torch
import torch.nn as nn
import torch.autograd.profiler as profiler
import warnings
from torch.utils.tensorboard import SummaryWriter
from torch.optim.lr_scheduler import ReduceLROnPlateau
from tqdm import tqdm
import numpy as np
from sklearn.metrics import accuracy_score, f1_score, classification_report
from sklearn.linear_model import LinearRegression

import matplotlib.pyplot as plt
from matplotlib.lines import Line2D
from scipy.spatial import ConvexHull
from scipy.linalg import solve_triangular
from datetime import datetime
import pickle

#from networks.MGDA import *
from torch.autograd import Variable

#from modules.data_tools import *
from data_classes import *
from networks import *
from utils import *
from Communication import *
from fbm import fbm
import warnings


print("Done with imports")

MODEL_PATH = "alpha_3D_e5_5_50_28-Oct_(11-49-54)"
TRAJ_PATH = "beads.csv"
EXPORT_FOLDER = "/export"

# Network characteristics
models = [0,1,2,3,4]
batch_size_test = 16
n_c = 64
latent_dim = 32
num_workers = 1
dim = 3
lr = 1e-3
x_dim = 20
e_dim = 24

if __name__ == "__main__":
    #with warnings.catch_warnings():
        #warnings.simplefilter("error")
        # Instanciate the network
        model = TrajsNet(n_classes=len(models),n_c = n_c, lr=lr,latent_dim=latent_dim,p=0.,
                     save_path = "",dim=dim,
                     x_dim=x_dim, e_dim = e_dim)# Load previously learnt weights
        print(model.load_state_dict(torch.load(MODEL_PATH)))
        model.eval()
        print("Model loaded, waiting to receive data")

        # Test sur des trajectoires factices

        #trajectories = [torch.from_numpy(np.random.normal(size=(10,3))) for k in range(10)]

        """
        trajectories, _, _ = trajs_from_ascii(TRAJ_PATH)
        alphas = len(trajectories) * [1]
        ds = TrajListDataset(trajectories, alphas)
        assert trajectories[0].shape[1] == 3, "Traj should have 3 columns but has %d" % trajectories[0].shape[1]
        assert ds[0].x.shape[1] == x_dim, "x should have %d columns but has %d" % (x_dim, ds[0].x.shape[1])
        assert ds[0].edge_attr.shape[1] == e_dim, "edge_attr should have %d columns but has %d" % (e_dim, ds[0].edge_attr.shape[1])

        dl = DataLoader(ds, batch_size=batch_size_test, shuffle=False,num_workers=1, drop_last=False)
        _, alpha, _, _, _, _, _ = model.run_epoch(dl)

        alpha = alpha.detach().cpu().numpy()
        alpha_coeff = np.clip(alpha[:,0],0.,2.)
        alpha_coeff[np.isnan(alpha_coeff)] = 1.

        print(alpha_coeff)
        """

        # Tant que le programme est en vie
        while True:

            # Attendre un fichier
            bytes_array = ReceiveData()
            array = convertBytesToFloats(bytes_array)
            print("Received Array")
            array = np.array(array)
            print(array.shape)

            # Le lire
            trajectories, df, traj_ids = trajs_from_array(array)
            # Read trajectories from file
            # df is the data table with all columns
            # traj_ids is the list of ids selected for inference (those matching length requirement)
            #trajectories, df, traj_ids = trajs_from_ascii(TRAJ_PATH)
            # Dummy alpha values to fill the dataset (it needs a "true alpha" value, in case it's used for training)
            alphas = len(trajectories) * [1]
            # Instanciate the dataset
            ds = TrajListDataset(trajectories, alphas)
            print(trajectories[0])
            assert trajectories[0].shape[1] == 3, "Traj should have 3 columns but has %d" % trajectories[0].shape[1]
            assert ds[0].x.shape[1] == x_dim, "x should have %d columns but has %d" % (x_dim, ds[0].x.shape[1])
            assert ds[0].edge_attr.shape[1] == e_dim, "edge_attr should have %d columns but has %d" % (e_dim, ds[0].edge_attr.shape[1])

            dl = DataLoader(ds, batch_size=batch_size_test, shuffle=False,num_workers=1, drop_last=False)

            print("Created dataloader")

            # Perform inference
            _, alpha, _, _, _, _, _ = model.run_epoch(dl)

            # Extract infered diffusion coefficients and clip their values to valid intervals
            alpha = alpha.detach().cpu().numpy()
            alpha_coeff = np.clip(alpha[:,0],0.,2.)
            alpha_coeff[np.isnan(alpha_coeff)] = 1.
            print("Inference done")

            # Fill dataframe with result column
            df["alpha"] = 1.
            for i, traj_id in enumerate(traj_ids):
                df.loc[df.trajectory == traj_id,"alpha"] = alpha_coeff[i]
            print("Dataframe filled")
            # Export dataframe
            # df.to_csv("%s/processed_file.csv" % EXPORT_FOLDER, sep="\t", header=False, index=False)
            print("Exporting data")
            transmitData(df[["id","alpha"]].values.T)
            print("Transmitted data !")
