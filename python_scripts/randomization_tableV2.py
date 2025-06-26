# -*- coding: utf-8 -*-
"""
Created on Thu Jun 26 15:25:46 2025

@author: lilux
"""
import random, pandas as pd

# Charger l'ancien tableau
df = pd.read_csv("randomization_table.csv")

# Nombre de nouveaux participants (20 de plus)
n_new = 20

# Low = 1, High = 2
orders = [(1, 2)]*(n_new//2) + [(2, 1)]*(n_new//2)
random.shuffle(orders)

# Créer et ajouter les nouvelles lignes
new_df = pd.DataFrame({
    "Participant": range(len(df)+1, len(df)+1+n_new),
    "Session 1": [o[0] for o in orders],
    "Session 2": [o[1] for o in orders]
})
result = pd.concat([df, new_df], ignore_index=True)

# Sauvegarde
result.to_csv("randomization_table.csv", index=False)
print(result)
