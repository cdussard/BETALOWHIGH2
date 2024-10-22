# -*- coding: utf-8 -*-
"""
Created on Wed Apr 14 17:25:20 2021

@author: cldus
"""
import serial
import time
from pylsl import StreamInlet, resolve_stream


#32773: 27,#"OVTK_StimulationId_TrialStart"

port = "COM7"
serialPort = serial.Serial(port, 115200, timeout=1)
print("Port initialized.")
time.sleep(2)

def testArduino(trigger):
    for i in range(2):
        print("before write")
        val = serialPort.write(trigger.to_bytes(1,byteorder='little'))
        print("after write ", val)
        time.sleep(1)
        serialPort.write(0)

def testArduinoLSL(trigger):
    streams = resolve_stream('type', 'stimEEG')
    print("======streams========")
    inlet = StreamInlet(streams[0])
    print(inlet.info().as_xml())
    count_nb_test = 3
    while count_nb_test>0:
        sample, timestamp = inlet.pull_sample()    
        stim = int(sample[1])
        print("stim:",stim)
        if stim!=0:
            if stim == 32773:
                print("before write")
                val = serialPort.write(trigger.to_bytes(1,byteorder='little'))
                print("after write ", val)
                time.sleep(2)
                count_nb_test -= 1


#testArduino(30)
testArduinoLSL(10)

serialPort.close()
                
print("End.")
