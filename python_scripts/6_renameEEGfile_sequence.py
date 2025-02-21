# -*- coding: utf-8 -*-
"""
Created on Fri Feb 21 19:31:16 2025

@author: claire.dussard
"""

import os

path_to_sequence = "../csv_files/"
path_to_eeg = "../../rawdata/openvibe/"

# Lecture du numéro de run dans le fichier log
try:
    with open(os.path.join(path_to_sequence, 'filenogotoread.txt'), 'r') as file:
        run_number = str(int(file.read().strip()) - 1)  # pck il sera incrémenté
except FileNotFoundError:
    print("Fichier de log non trouvé. Assurez-vous que 'filenogotoread.txt' existe.")
    exit(1)

for ext in [".eeg", ".vhdr", ".vmrk"]:
    # Construction du chemin complet du fichier source
    old_filepath = os.path.join(path_to_eeg, "6_neurofeedback_gonogo" + ext)
    
    # Construction du nouveau nom et du chemin complet de destination
    new_filename = f"6_neurofeedback_gonogo_run{run_number}{ext}"
    new_filepath = os.path.join(path_to_eeg, new_filename)
    
    # Vérification de l'existence du fichier source et renommage
    if os.path.exists(old_filepath):
        os.rename(old_filepath, new_filepath)
        print(f"Renommé avec succès '{old_filepath}' en '{new_filepath}'.")
    else:
        print(f"Fichier de sortie '{old_filepath}' non trouvé.")


#===============
import os
import re

path_to_sequence = "../csv_files/"
path_to_eeg = "../../rawdata/openvibe/"

# Lecture du numéro de run dans le fichier log
try:
    with open(os.path.join(path_to_sequence, 'filenogotoread.txt'), 'r') as file:
        run_number = str(int(file.read().strip()) - 1)  # car il sera incrémenté
except FileNotFoundError:
    print("Fichier de log non trouvé. Assurez-vous que 'filenogotoread.txt' existe.")
    exit(1)

# Renommage des fichiers (.eeg, .vhdr, .vmrk)
for ext in [".eeg", ".vhdr", ".vmrk"]:
    old_filepath = os.path.join(path_to_eeg, "6_neurofeedback_gonogo" + ext)
    new_filename = f"6_neurofeedback_gonogo_run{run_number}{ext}"
    new_filepath = os.path.join(path_to_eeg, new_filename)
    
    if os.path.exists(old_filepath):
        os.rename(old_filepath, new_filepath)
        print(f"Renommé avec succès '{old_filepath}' en '{new_filepath}'.")
    else:
        print(f"Fichier '{old_filepath}' non trouvé.")

# Mise à jour du contenu des fichiers header (.vhdr et .vmrk)
for ext in [".vhdr", ".vmrk"]:
    header_filepath = os.path.join(path_to_eeg, f"6_neurofeedback_gonogo_run{run_number}{ext}")
    try:
        with open(header_filepath, 'r', encoding='utf-8') as file:
            content = file.read()
    except FileNotFoundError:
        print(f"Fichier header '{header_filepath}' non trouvé.")
        continue

    # On remplace d'abord l'ancien identifiant par le nouveau
    old_identifier = "6_neurofeedback_gonogo"
    new_identifier = f"6_neurofeedback_gonogo_run{run_number}"
    updated_content = content.replace(old_identifier, new_identifier)
    
    # Ensuite, pour les lignes DataFile et MarkerFile, on retire le chemin s'il y en a un.
    # Cette expression régulière capture tout ce qui précède le nom de fichier (après DataFile= ou MarkerFile=)
    updated_content = re.sub(r"(DataFile\s*=).*[\\/](.*)", r"\1\2", updated_content)
    updated_content = re.sub(r"(MarkerFile\s*=).*[\\/](.*)", r"\1\2", updated_content)
    
    # Sauvegarde du fichier header modifié
    with open(header_filepath, 'w', encoding='utf-8') as file:
        file.write(updated_content)
    
    print(f"Fichier header '{header_filepath}' mis à jour avec succès.")
