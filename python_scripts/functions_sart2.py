# -*- coding: utf-8 -*-
"""
Created on Mon Apr 29 18:18:42 2024

@author: claire.dussard
"""

import random
import string
import numpy as np
import pandas as pd


def generate_sequence(length, s_count, noGoCharacter):#maximum 3 consecutive
    # Check if the requested number of 'S' is achievable
    print(s_count)
    if s_count * 3 - 1 > length:
        raise ValueError("Impossible to generate a sequence with so many 'S' letters.")

    # Generate a list of 'length' random letters from the alphabet (except 'S')
    alphabet = string.ascii_uppercase.replace(noGoCharacter, '')  # Alphabet without the letter 'S'
    sequence = random.choices(alphabet, k=length)

    # Place the 's_count' 'S' letters randomly in the sequence
    while True:
        # Reset the sequence
        sequence = random.choices(alphabet, k=length)
        consecutive_count = 100
        # Place the 'S' letters in the sequence
        while consecutive_count>2:
            s_indices = random.sample(range(length), s_count)
            s_indices.sort()
            consecutive_count = 0
            for i in range(len(s_indices) - 1):
                if s_indices[i + 1] - s_indices[i] == 1:
                    consecutive_count += 1
        valid_sequence = True
        for i, index in enumerate(s_indices):
            if i > 1 and index == s_indices[i - 1] + 1 and index == s_indices[i - 2] + 2:
                # Check if there are more than two consecutive 'S' instances
                valid_sequence = False
                break
            sequence[index] = noGoCharacter

        # Check if the sequence is valid
        if valid_sequence:
            break

    # Convert the list into a string
    sequence_string = ''.join(sequence)

    # Check if there are enough 'S' characters
    if sequence_string.count(noGoCharacter) != s_count:
        raise ValueError("Generated sequence does not contain the requested number of 'S' letters.")

    return sequence_string

   

def generate_numbers(n_runs, minNoGoNumberPerRun, maxNoGoNumberPerRun, targetTotalNoGo):
    while True:
        # Déterminer 4 nombres aléatoires entre 15 et 21 inclusivement
        numbers = [random.randint(minNoGoNumberPerRun, maxNoGoNumberPerRun) for _ in range(n_runs-1)]
        # Calculer la somme de ces nombres
        total = sum(numbers)
        # Calculer le cinquième nombre pour que la somme soit égale à targetTotalNoGo
        fifth_number = targetTotalNoGo - total
        # Vérifier si le cinquième nombre est dans la plage min-max
        if minNoGoNumberPerRun <= fifth_number <= maxNoGoNumberPerRun:
            # Ajouter le cinquième nombre à la liste et retourner les nombres
            numbers.append(fifth_number)
            return numbers


def generate_sequence_with_mean(n_trials,minDuration,maxDuration, tolerance):
    sequence = np.random.uniform(minDuration, maxDuration, n_trials)
    return sequence


def generate_full_seq(n_runs,n_trials,tolerance,minNoGoNumberPerRun,maxNoGoNumberPerRun,minDuration,maxDuration,noGoCharacter,numSujet,numSession):
    targetMeanNoGoNumber = np.mean([minNoGoNumberPerRun, maxNoGoNumberPerRun])
    targetTotalNoGo = targetMeanNoGoNumber *  n_runs
    result = generate_numbers(n_runs,minNoGoNumberPerRun,maxNoGoNumberPerRun,targetTotalNoGo)
    #generate sequences
    liste_sequences = []
    total_noGo_count = 0
    while total_noGo_count!=targetTotalNoGo:
        liste_sequences = []
        for n_NoGo in result:
            seq = generate_sequence(n_trials, int(n_NoGo),noGoCharacter)
            liste_sequences.append(seq)
        print(liste_sequences)
        aplat = ''.join(liste_sequences)
        total_noGo_count = aplat.count(noGoCharacter)
        print("au total : "+str(total_noGo_count)+"\n mais tu voulais "+str(targetTotalNoGo))
    liste_sequences = [list(string) for string in liste_sequences]
    #generate durations
    targetMeanDuration = np.mean([minDuration, maxDuration])
    liste_duration = []
    for i in range(n_runs):
        current_mean = 0
        while abs(current_mean - targetMeanDuration) > tolerance:
            print("generating durations...")
            durations = generate_sequence_with_mean(n_trials,minDuration,maxDuration,tolerance)
            current_mean = np.mean(durations)
        liste_duration.append(durations)     
    data = []
    for i in range(n_runs):
        j = 0
        for (charac,duration) in zip(liste_sequences[i],liste_duration[i]):
            data.append([numSujet,numSession,i+1,j+1,charac,charac==noGoCharacter, duration])
            j = j + 1
    df = pd.DataFrame(data, columns=['num_sujet','num_session','run','trial', 'character', 'isNoGo','duration'])    
    return df
