# -*- coding: utf-8 -*-
"""
Created on Fri Feb 21 17:42:49 2025

@author: lilux
"""
import random
import pandas as pd

# Nombre de participants
n_participants = 20

# Création d'une liste équilibrée des ordres possibles
orders = [(1, 2)] * (n_participants // 2) + [(2, 1)] * (n_participants // 2)
random.shuffle(orders)  # Mélange aléatoire des ordres

# Création d'un DataFrame pandas
randomization_table = pd.DataFrame({
    "Participant": list(range(1, n_participants + 1)),
    "Session 1": [order[0] for order in orders],
    "Session 2": [order[1] for order in orders]
})

# Sauvegarde en CSV
randomization_table.to_csv("randomization_table.csv", index=False)

print(randomization_table)
