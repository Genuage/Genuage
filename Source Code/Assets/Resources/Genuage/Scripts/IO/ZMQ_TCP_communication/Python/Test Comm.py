import numpy as np
import zmq
import time


context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")
print("cutting connection")
socket.close()
