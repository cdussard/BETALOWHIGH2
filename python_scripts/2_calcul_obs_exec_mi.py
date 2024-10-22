# -*- coding: utf-8 -*-
"""
Created on Mon Oct 21 17:28:23 2024

@author: claire.dussard
"""

import pandas as pd
import os


print(os.getcwd())

rel_path = "../../rawdata/csv_files/"

path = os.getcwd()


# Liste des fichiers (sans extension)
files = ["rest_low", "rest_high", "obs_low", "obs_high", "MI_low", "MI_high", "exec_low", "exec_high"]


#version ratios
# Liste pour stocker les résultats
results = []

# Dictionnaire pour stocker les valeurs de 'rest' pour chaque bande
rest_values = {}

# Boucle sur chaque fichier pour calculer les statistiques et potentiellement les ratios
for file in files:
    # Crée le chemin complet du fichier CSV
    filename = rel_path + file + '.csv'
    csv_filename = os.path.join(path, filename)
    
    # Lis le fichier CSV
    df = pd.read_csv(csv_filename)
    data = df.iloc[:, 2]  # Utilise uniquement la 3ème colonne (index 2)
    
    # Déduis la task et la bande à partir du nom du fichier
    task, bande = file.split('_')
    
    # Calcul des statistiques pour la colonne
    min_val = data.min()
    max_val = data.max()
    mean_val = data.mean()
    median_val = data.median()
    stdev_val = data.std()  # Écart-type des données brutes
    
    # Stocker les résultats bruts
    results.append({
        'task': task,
        'bande': bande,
        'min': min_val,
        'max': max_val,
        'mean': mean_val,
        'median': median_val,
        'stdev': stdev_val,
        'ratio': False  # False pour les données originales
    })
    
    # Si la tâche est 'rest', on stocke les valeurs pour le ratio
    if task == 'rest':
        rest_values[bande] = data  # Stocke directement toutes les données pour calculer les ratios plus tard
    
    # Si la tâche est 'obs', 'MI', ou 'exec', on calcule les ratios par rapport à 'rest'
    if task in ['obs', 'MI', 'exec']:
        # Récupère les valeurs de 'rest' pour la même bande
        rest_data = rest_values[bande]
        
        # Calcul des ratios point par point pour toutes les valeurs
        ratios = data / rest_data
        
        # Calcul des statistiques des ratios
        ratio_min = ratios.min()
        ratio_max = ratios.max()
        ratio_mean = ratios.mean()
        ratio_median = ratios.median()
        ratio_stdev = ratios.std()  # Écart-type des ratios
        
        # Stocker les résultats avec les ratios
        results.append({
            'task': task,
            'bande': bande,
            'min': ratio_min,
            'max': ratio_max,
            'mean': ratio_mean,
            'median': ratio_median,
            'stdev': ratio_stdev,
            'ratio': True  # True pour les données des ratios
        })

# Création d'un DataFrame avec les résultats
summary_df = pd.DataFrame(results)

# Écriture des résultats dans un fichier Excel
summary_df.to_excel(rel_path + 'summary_statistics_with_ratios_and_stdev.xlsx', index=False)