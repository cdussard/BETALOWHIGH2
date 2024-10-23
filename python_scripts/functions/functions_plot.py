# -*- coding: utf-8 -*-
"""
Created on Tue Oct 22 16:37:19 2024

@author: claire.dussard
"""

import mne
import matplotlib.pyplot as plt

def plot_func(data,axs,fmin,fmax,isEpoch):
    tmin = 0
    tmax= 130
    if isEpoch:
        data.plot_psd(ax=axs[0],fmin=fmin,fmax=fmax,tmin = tmin,tmax=tmax,dB=False,spatial_colors=True,picks=["C3","FC1","CP5","FC5","CP1","C4"])
    else:
        mne.viz.plot_raw_psd(data,ax=axs[0],fmin=fmin,fmax=fmax,tmin = tmin,tmax=tmax,dB=False,spatial_colors=True,picks=["C3","FC1","CP5","FC5","CP1","C4"])#OPENVIBE
    data_C3 = data.get_data(picks=['C3'])
    data_CP1 = data.get_data(picks=['CP1'])
    data_CP5 = data.get_data(picks=['CP5'])
    data_FC1 = data.get_data(picks=['FC1'])
    data_FC5 = data.get_data(picks=['FC5'])
    
    laplacianC3_data = data_C3 - 1/4*(data_CP1+data_CP5+data_FC1+data_FC5)
    
    info_laplacian = mne.create_info(["laplacian_c3"], 1000, ch_types='eeg', verbose=None)
    
    if isEpoch:
        laplacianC3_data = laplacianC3_data[0]
    rawLaplacian_C3 = mne.io.RawArray(laplacianC3_data,info_laplacian)
    
    if isEpoch:
       rawLaplacian_C3.plot_psd(ax=axs[1],fmin=fmin,fmax=fmax,tmin = tmin,tmax=tmax,dB=False)  
    else:
        mne.viz.plot_raw_psd(rawLaplacian_C3,ax=axs[1],fmin=fmin,fmax=fmax,tmin = tmin,tmax=tmax,dB=False)    
    
    info_C3 = mne.create_info(["C3"], 1000, ch_types='eeg', verbose=None)
    if isEpoch:
        data_C3 = data_C3[0]
    rawmoy_C3 = mne.io.RawArray(data_C3,info_C3)
    
    if isEpoch:
        rawmoy_C3.plot_psd(ax=axs[2],fmin=fmin,fmax=fmax,tmin = tmin,tmax=tmax,dB=False)    
    else:
        mne.viz.plot_raw_psd(rawmoy_C3,ax=axs[2],fmin=fmin,fmax=fmax,tmin = tmin,tmax=tmax,dB=False)    
        
        
        
def compare_power(all_data,fmin,fmax,pick):
    tmin = 0
    tmax= 130
    fig, axs = plt.subplots(1, 2, figsize=(18, 10))
    cols = ["black","red","blue","darkgreen"]
    for i in range(len(all_data)):
        data = all_data[i]
        data_C3 = data.get_data(picks=[pick])
        info_C3 = mne.create_info(["C3"], 1000, ch_types='eeg', verbose=None)
        data_C3_ = data_C3[0]
        rawmoy_C3 = mne.io.RawArray(data_C3_,info_C3)
        rawmoy_C3.plot_psd(ax=axs[0],fmin=fmin,fmax=fmax,tmin = tmin,tmax=tmax,dB=False,color=cols[i] ,spatial_colors=False)
        
        data_C3 = data.get_data(picks=['C3'])
        data_CP1 = data.get_data(picks=['CP1'])
        data_CP5 = data.get_data(picks=['CP5'])
        data_FC1 = data.get_data(picks=['FC1'])
        data_FC5 = data.get_data(picks=['FC5'])
        
        laplacianC3_data = data_C3 - 1/4 * (data_CP1 + data_CP5 + data_FC1 + data_FC5)
        laplacianC3_data_ = laplacianC3_data[0]
        info_laplacian = mne.create_info(["laplacian_C3"], 1000, ch_types='eeg', verbose=None)
        rawLaplacian_C3 = mne.io.RawArray(laplacianC3_data_,info_laplacian)
        rawLaplacian_C3.plot_psd(ax=axs[1],fmin=fmin,fmax=fmax,tmin = tmin,tmax=tmax,dB=False,color=cols[i],spatial_colors=False)
        
        
    
    
    
    
def filter_data(data):
    pass_eeg = data.copy().filter(0.1,250, method='iir', iir_params=dict(ftype='butter', order=4))
    filtered = pass_eeg.notch_filter(freqs=[50,100], filter_length='auto',phase='zero')
    filtered.set_channel_types({'EMG': 'emg'}) 
    filtered.set_channel_types({'ECG': 'ecg'}) 
    montageEasyCap = mne.channels.make_standard_montage('easycap-M1')
    filtered.set_montage(montageEasyCap)
    return filtered