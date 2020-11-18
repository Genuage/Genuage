import pandas as pd
import numpy as np
import fbm
import torch
from tqdm import tqdm

TRAJ_COL = 6
ID_COL = 0
X_COL = 1
Y_COL = 2
Z_COL = 3
T_COL = 5

def trajs_from_df(df):
    traj_index = df["trajectory"].unique().tolist()
    trajs = []
    indices = []
    for k in tqdm(traj_index):
        traj = df[df.trajectory == k].sort_values(by="time",ascending=True)[["x","y","z"]].to_numpy()
        if traj.shape[0] >= 5:
            trajs.append(torch.from_numpy(traj))
            indices.append(k)
    print("Kept %d trajectories" % len(trajs))
    return trajs, df, indices

def trajs_from_ascii(f_path):
    df = pd.read_csv(f_path, engine="python",  index_col="id",header=0, sep="; ",decimal=",")
    return trajs_from_df(df)

def trajs_from_array(array):
    df = pd.DataFrame(data=array.T)
    df = df.rename(columns={ID_COL:"id", TRAJ_COL:"trajectory",X_COL:"x",Y_COL:"y",Z_COL:"z",T_COL:"time"})
    df["trajectory"] = df["trajectory"].astype(int)
    return trajs_from_df(df)

def make_fbm(alpha, N=12,dim=3):
    """
    Generate FBM trajectories if needed
    """
    H = alpha/2
    return torch.from_numpy(np.stack([fbm(N,H) for k in range(dim)],axis=1)).double()
