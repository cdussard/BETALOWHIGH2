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

#faire pour tous les sujets ? 

# for i in range(20):
#     print(i)
#     df_NF = generate_full_seq(n_runs,n_trials,tolerance,minNoGoNumberPerRun,maxNoGoNumberPerRun,minDuration,maxDuration,noGoCharacter,i+1,2)
#     df_NF.to_csv("csv_files/sequenceV2_sujet"+str(i+1)+".csv",index=False)
    
for session in range(1,3):
    for num_sujet in range(1,21):
        df_NF,seq_nf = generate_full_seq(n_runs,n_trials,tolerance,minNoGoNumberPerRun,maxNoGoNumberPerRun,minDuration,maxDuration,noGoCharacter,num_sujet,session)
        df_OBS,seq_obs = generate_full_seq(n_runs,n_trials,tolerance,minNoGoNumberPerRun,maxNoGoNumberPerRun,minDuration,maxDuration,noGoCharacter,num_sujet,session)
        
        for i in range(n_runs):
            df_NF_run = df_NF[df_NF["run"] == i + 1].copy()
            df_NF_run["type"] = "NF"
            df_OBS_run = df_OBS[df_OBS["run"] == i + 1].copy()
            df_OBS_run["type"] = "OBS"
            df_run_i = pd.concat([df_NF_run, df_OBS_run], ignore_index=True)
            df_run_i.to_csv(f"../gonogo_sequences/sequence_{i+1}_sujet_{num_sujet}_session_{session}.csv", index=False)
    


# #vérif
import re

# Count occurrences of 'S' and longest consecutive 'S' sequence
results = []
for seq_type in [seq_nf,seq_obs]:
    for seq in seq_type:
        count_s = seq.count("S")
        longest_s = max((len(m.group()) for m in re.finditer(r"S+", seq)), default=0)
        results.append((count_s, longest_s))
print(results)