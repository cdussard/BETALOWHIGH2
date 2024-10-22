# -*- coding: utf-8 -*-
"""
Created on Mon Aug 21 10:48:07 2023

@author: claire.dussard
"""

import random
import time

from pylsl import StreamInfo, StreamOutlet

info = StreamInfo(name='type', type='EEG')

  # next make an outlet
outlet = StreamOutlet(info)

print("now sending markers...")
markernames = []
while True:
    # pick a sample to send an wait for a bit
    outlet.push_sample([random.choice(markernames)])
    time.sleep(random.random() * 3)