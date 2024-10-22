# -*- coding: utf-8 -*-
"""
Created on Tue Oct 22 15:48:37 2024

@author: claire.dussard
"""

import mne
import os
import numpy as np
import matplotlib.pyplot as plt
import matplotlib 
from functions.functions_plot import *


print(os.getcwd())

rel_path = "../../rawdata/eeg/"

path = os.getcwd()

date = "NFPARK_2024-10_16"

chemin_rest = date + "_3"+".vhdr"
chemin_MItraining = date + "_2-b"+".vhdr"
    

path_rest = rel_path + chemin_rest
path_MItraining = rel_path + chemin_MItraining  

data_rest = mne.io.read_raw_brainvision(path_rest,preload=True,eog=('VEOG', 'HEOG'))
mne.rename_channels(data_rest.info,{'Fcz':'FCz'})
data_filtered_rest = filter_data(data_rest)


data_MItraining = mne.io.read_raw_brainvision(path_MItraining,preload=True,eog=('VEOG', 'HEOG'))
mne.rename_channels(data_MItraining.info,{'Fcz':'FCz'})
data_filtered_mi = filter_data(data_MItraining)


event_rest = {'Début rest':112}
annotations_rest = mne.events_from_annotations(data_rest)
dureePreEpoch = 3
dureeEpoch = 35

epochs_rest = mne.Epochs(data_filtered_rest,annotations_rest[0],event_rest,tmin=-dureePreEpoch,tmax = dureeEpoch,baseline=None, preload=True)

fig_rest, axs_rest = plt.subplots(3, 1, figsize=(18, 10))

plot_func(epochs_rest,axs_rest,1,dureeEpoch,True)

epochs_rest.get_data(picks=['C3'])



fig_rest, axs_rest = plt.subplots(3, 1, figsize=(18, 10))
plot_func(data_rest,axs_rest,1,60,False)

fig_mi, axs_mi = plt.subplots(3, 1, figsize=(18, 10))

plot_func(data_MItraining,axs_mi,1,60,False)





#pour les MI OBS EXEC
event_main = {'Affichage main':124}
annotations_mi = mne.events_from_annotations(data_MItraining)

epochs_exec_mi_obs = mne.Epochs(data_filtered_mi,annotations_mi[0],event_main,tmin=-dureePreEpoch,tmax = dureeEpoch,baseline=None, preload=True)


fig_obs, axs_obs = plt.subplots(3, 1, figsize=(18, 10))
plot_func(epochs_exec_mi_obs[0],axs_obs,1,dureeEpoch,True)

fig_exec, axs_exec = plt.subplots(3, 1, figsize=(18, 10))
plot_func(epochs_exec_mi_obs[1],axs_exec,1,dureeEpoch,True)

fig_mi, axs_mi = plt.subplots(3, 1, figsize=(18, 10))
plot_func(epochs_exec_mi_obs[2],axs_mi,1,dureeEpoch,True)


liste = [epochs_rest,epochs_exec_mi_obs[0],epochs_exec_mi_obs[1],epochs_exec_mi_obs[2]]

compare_power(liste,1,60,"C3")#a faire aussi avec laplacien