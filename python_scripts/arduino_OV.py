"""
Created on Thu Dec 10 17:34:47 2020

@author: cldus
"""

import serial
from pylsl import StreamInlet, resolve_stream,resolve_streams

#getDict
def getDict():#fait le lien entre l'entier recu (stim) et le label de la stim (a droite)
    return {
        0: "BUG ARDUINO",
        13: "OVTK_StimulationId_Number_0D",
        16: "OVTK_StimulationId_Number_10",
        17: "OVTK_StimulationId_Number_11",
        18: "OVTK_StimulationId_Number_12",
        19:"OVTK_StimulationId_Number_13",
        20:"OVTK_StimulationId_Number_14",
        260: "OVTK_GDF_Artifact_Movement",
        800: "OVTK_GDF_End_Of_Trial",
        1010: "OVTK_GDF_End_Of_Session",
        32769: "OVTK_StimulationId_ExperimentStart",
        32770: "OVTK_StimulationId_ExperimentStop",
        32771: "OVTK_StimulationId_SegmentStart",
        32775: "OVTK_StimulationId_BaselineStart",
        32779: "OVTK_StimulationId_VisualStimulationStart",
        32780: "OVTK_StimulationId_VisualStimulationStop",
        33034: "OVTK_StimulationId_Label_0A",
        33035: "OVTK_StimulationId_Label_0B",
        33036: "OVTK_StimulationId_Label_0C",
        33037: "OVTK_StimulationId_Label_0D",
        33051: "OVTK_StimulationId_Label_1B",
        33054: "OVTK_StimulationId_Label_1E",
        33055: "OVTK_StimulationId_Label_1F"
}


#getDictInt
def getDictInt():#TOUS VERIFIER LES COMMENTAIRES
    return {
  0: 101, #"test Arduino"
  13: 113, #"OVTK_StimulationId_Number_0D"
  16: 126, #"OVTK_StimulationId_Number_10"
  17: 127, #"OVTK_StimulationId_Number_11"
  18: 128, #"OVTK_StimulationId_Number_12"
  19:132,#OVTK_StimulationId_Number_13
  20:133,#OVTK_StimulationId_Number_14
  260: 105, #"OVTK_GDF_Artifact_Movement"
  800: 106, #"OVTK_GDF_End_Of_Trial"
  1010: 129, #"OVTK_GDF_End_Of_Session"
  32769: 110, #"OVTK_StimulationId_ExperimentStart"
  32770: 111, #"OVTK_StimulationId_ExperimentStop"
  32771: 122, #"OVTK_StimulationId_SegmentStart"
  32775: 121, #"OVTK_StimulationId_BaselineStart"
  32779: 112, #"OVTK_StimulationId_VisualStimulationStart"
  32780: 114, #"OVTK_StimulationId_VisualStimulationStop"
  33034: 116, #"OVTK_StimulationId_Label_0A"
  33035: 117, #"OVTK_StimulationId_Label_0B"
  33036: 118, #"OVTK_StimulationId_Label_0C"
  33037: 119, #"OVTK_StimulationId_Label_0D"
  33051: 124, #"OVTK_StimulationId_Label_1B"
  33054: 130,#OVTK_StimulationId_Label_1E
  33055:131,#OVTK_StimulationId_Label_1F

}

def envoiTriggers_LSL(mode,port):
    dicInt = getDictInt()
    dicString = getDict()
    if mode=="arduino": # Windows : partie arduino
        serialPort = serial.Serial(port, 115200, timeout=1)
        print("found port")
    # streams = resolve_streams()
    # for stream in streams:  
    #     print(stream.name)
    streams = resolve_stream('type','EEG')
    print("======streams========")
    inlet = StreamInlet(streams[0])
    print(inlet.info().as_xml())
    if mode=="test":#mode test
        while True:
            sample, timestamp = inlet.pull_sample()
            if sample[1] != 0:
                stim = int(sample[1])
                if stim == 1010:
                    print ("ARRET DE LA BOUCLE")
                    break
                elif stim ==-1 or stim == 1:
                    print("RECEPTION DE -1 STIM : RECONNECTER L'ACQUISITION SERVER")
                    break
                else:#stimInt = dicInt[stim]
                    try:
                        stimString = dicString[stim]#print(stimInt)
                        print(stimString)#appel au dictionnaire au lieu de if
                        print(str(stim))#appel au dictionnaire au lieu de if
                    except KeyError:
                        print("RECEPTION DE MAUVAISE STIM : RECONNECTER L'ACQUISITION SERVER")
                        serialPort.close()
                        break

    #mode arduino
    elif mode =="arduino":  
        print("arduino")
        while True:
            sample, timestamp = inlet.pull_sample()
            if sample[1] != 0:
                print(sample)
                stim = int(sample[1])         
                if stim == 1010:
                    serialPort.close()
                    print ("ARRET DE LA BOUCLE")
                    break
                elif stim ==-1 or stim == 1:
                    print("RECEPTION DE -1 STIM : RECONNECTER L'ACQUISITION SERVER")
                    serialPort.close()
                    break
                else:
                    try:
                        print("stim",stim)
                        stim = dicInt[stim]                
                        serialPort.write(stim.to_bytes(1,byteorder='little'))
                    except KeyError:
                        print("RECEPTION DE MAUVAISE STIM : RECONNECTER L'ACQUISITION SERVER")
                        serialPort.close()
                        break
                        
                    #time.sleep(0.002)#a modifier apres code laurent
                    #serialPort.write(byte0)     
    #serialPort.close()

envoiTriggers_LSL("test",'COM7')
#envoiTriggers_LSL("arduino",'COM7')