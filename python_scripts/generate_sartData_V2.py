# -*- coding: utf-8 -*-
"""
Created on Mon Apr 29 18:06:36 2024

@author: claire.dussard
"""


import random
import string
import numpy as np
import pandas as pd

from functions_sart2 import generate_full_seq

n_runs = 10
n_trials = 60
minNoGoNumberPerRun = 10
maxNoGoNumberPerRun = 20
noGoCharacter = "S"
minDuration = 1#0.6 
maxDuration = 0.6# 0.4#0.6
tolerance = 0.01
numeric = False




for i in range(20):
    print(i)
    df_NF = generate_full_seq(n_runs,n_trials,tolerance,minNoGoNumberPerRun,maxNoGoNumberPerRun,minDuration,maxDuration,noGoCharacter,i+1,2)
    df_NF.to_csv("csv_files/sequenceV2_sujet"+str(i+1)+".csv",index=False)
    


num_sujet = 1
session = 1
df_NF_s01,seq_nf = generate_full_seq(n_runs,n_trials,tolerance,minNoGoNumberPerRun,maxNoGoNumberPerRun,minDuration,maxDuration,noGoCharacter,num_sujet,session)
df_OBS_s01,seq_obs = generate_full_seq(n_runs,n_trials,tolerance,minNoGoNumberPerRun,maxNoGoNumberPerRun,minDuration,maxDuration,noGoCharacter,num_sujet,session)

for i in range(n_runs):
    df_run_i = df_NF_s01[df_NF_s01["run"]==i+1].append(df_OBS_s01[df_OBS_s01["run"]==i+1])
    df_run_i.to_csv("../csv_files/sequenceV2_"+str(i+1)+".csv",index=False)


# #vérif
# import re

# # Count occurrences of 'S' and longest consecutive 'S' sequence
# results = []
# for seq_type in [seq_nf,seq_obs]:
#     for seq in seq_type:
#         count_s = seq.count("S")
#         longest_s = max((len(m.group()) for m in re.finditer(r"S+", seq)), default=0)
#         results.append((count_s, longest_s))
#         print(results)