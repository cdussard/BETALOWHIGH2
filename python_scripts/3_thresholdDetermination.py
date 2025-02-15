# -*- coding: utf-8 -*-
"""
Created on Tue Jul  4 10:39:33 2023

@author: claire.dussard
"""
import pandas as pd
import numpy as np
import os

#path = "C:\manips\BETALOWHIGH\developpement_pilote/csv_files/"
path = "C:/Users/claire.dussard/OneDrive - ICM/Bureau/exp_highLowBeta/0_pilote/devpilote_BETALOWHIGH/csv_files/"
sep = ","
print(os.getcwd())
rel_path = "../csv_files/"

csv_filename = os.path.join(path, '5_tableau_moyennes.csv')
tableau_echantillons = pd.read_csv(csv_filename)

#IL FAUT AUSSI LIRE LES VALEURS DU BETA PARASITE

#lire les parametres a calculer
path_instructions = os.path.join(path, "config_neurofeedback.csv")
instructions = pd.read_csv(path_instructions)
percentilePireBeta = instructions["percentilePireBeta"]
percentileMeilleurBeta = instructions["percentileMeilleurBeta"]



echantillons = tableau_echantillons.iloc[:,2]
print("nb echantillons : "+str(len(echantillons)))
#a priori faire 25th percentile sur laisser stabiliser beta 1 min puis 
#1 min de repos  ==> voir dans quelle direction on a ajusté les valeurs
min_val = echantillons.min()
percentile_5 = np.percentile(echantillons, 5)
percentile_10 = np.percentile(echantillons, 10)
percentile_25 = np.percentile(echantillons, 25)
percentile_47 = np.percentile(echantillons,47)
percentile_50 = np.percentile(echantillons,50)
percentile_75 = np.percentile(echantillons,75)
percentile_pireBeta = np.percentile(echantillons,percentilePireBeta)
percentile_meilleurBeta = np.percentile(echantillons,percentileMeilleurBeta)
percentile_pireBetaParasite = #COMPLETER avec les valeurs du beta parasite (tableur ecrit par l'openvibe threshold)
percentileMeilleurBeta_parasite = #COMPLETER 

max_val = echantillons.max()

stdev = round(np.std(echantillons),3)


dict_threshold = {
    'min': min_val,
    'percentile5':percentile_5,
    'percentile10': percentile_10,
    'percentile25': percentile_25,
    'percentile47': percentile_47,
    'percentile50': percentile_50,
    'percentile75': percentile_75,   
    'percentilePireBeta': percentile_pireBeta,
    'percentileMeilleurBeta': percentile_meilleurBeta,
    'max': max_val,
    'stdev': stdev ,
    'percentilePireBeta_parasite': percentile_pireBetaParasite,
    'percentileMeilleurBeta_parasite': percentileMeilleurBeta_parasite       
                  
                  }

index = ['values']
dataframe = pd.DataFrame(dict_threshold,index=index)

print(dataframe)
csv_filename_output = os.path.join(path, 'values_threshold.csv')
dataframe.to_csv(csv_filename_output,index=None)