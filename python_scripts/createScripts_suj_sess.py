# -*- coding: utf-8 -*-
"""
Created on Fri Aug 25 17:08:56 2023

@author: claire.dussard
"""

import pandas as pd


allocationTypes = pd.read_csv("../secret/allocationTypes.csv")
mappingLowHigh = pd.read_csv("../secret/mappingLowHigh.csv")

intLow = mappingLowHigh["sessionLow"][0]
intHigh = mappingLowHigh["sessionHigh"][0]

numSujet = input("Sujet Number from 1-6 : ") #should be inputed by user
numSession = input("Session Number from 1-2 : ")#should be inputed by user

if numSession ==1: 
    type_sess_suj = allocationTypes[allocationTypes["num_sujet"]==numSujet]["session1"][0]
elif numSession ==2:
    type_sess_suj = allocationTypes[allocationTypes["num_sujet"]==numSujet]["session2"][0]




if type_sess_suj == intLow:
    with open("../../raccourcis/06_neurofeedbackLow.bat", "r") as file:
        script = file.read()
elif type_sess_suj == intHigh:
    with open("../../raccourcis/06_neurofeedbackHigh.bat", "r") as file:
        script = file.read()
with open(f"../../raccourcis/06_neurofeedback_participant{numSujet}session{numSession}.bat", "w") as file: 
    file.write(script)
    

if type_sess_suj == intLow:
    with open("../../raccourcis/051_thresholdDeterminationHigh.bat", "r") as file:
        script = file.read()
elif type_sess_suj == intHigh:
    with open("../../raccourcis/051_thresholdDeterminationLow.bat", "r") as file:
        script = file.read()
with open(f"../../raccourcis/051_thresholdDetermination_participant{numSujet}session{numSession}.bat", "w") as file: 
    file.write(script)