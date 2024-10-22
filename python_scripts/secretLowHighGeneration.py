# -*- coding: utf-8 -*-
"""
Created on Fri Aug 25 17:03:14 2023

@author: claire.dussard
"""

import pandas as pd
import random


intSessionLow = random.randint (1, 2) 
if intSessionLow ==1:
    intSessionHigh = 2
else:
    intSessionHigh = 1
    
df_sol = pd.DataFrame(columns=["sessionHigh","sessionLow"])
obj = {
       'sessionHigh':intSessionHigh,
       'sessionLow':intSessionLow
       }
df_sol = df_sol.append(obj,ignore_index=True)

df_sol.to_csv("../secret/mappingLowHigh.csv",index=False)
    

nbSujets = 6
nbSessions = 2

liste_sess_suj = [[1,2],[2,1],[1,2],[2,1],[1,2],[2,1]]
random.shuffle(liste_sess_suj)
random.shuffle(liste_sess_suj)
random.shuffle(liste_sess_suj)
random.shuffle(liste_sess_suj)


df_res = pd.DataFrame(columns=["num_sujet","session1","session2"])
for (num_sujet,res) in zip(range(nbSujets),liste_sess_suj):
    obj = {
        'num_sujet':(num_sujet+1),
        'session1': res[0],
        'session2': res[1]
        
        }
    df_res = df_res.append(obj,ignore_index=True)
    
    
df_res.to_csv("../secret/allocationTypes.csv",index=False)