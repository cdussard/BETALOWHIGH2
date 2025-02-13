# -*- coding: utf-8 -*-
"""
Created on Mon Apr 29 18:06:36 2024

@author: claire.dussard
"""


import random
import string
import numpy as np
import pandas as pd
from functions_sart2 import *

n_runs = 5
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
    df = generate_full_seq(n_runs,n_trials,tolerance,minNoGoNumberPerRun,maxNoGoNumberPerRun,minDuration,maxDuration,noGoCharacter,i+1,2)
    df.to_csv("csv_files/sequenceV2_sujet"+str(i+1)+".csv",index=False)
    
