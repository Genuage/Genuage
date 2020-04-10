import numpy as np
import zmq
import time


context = zmq.Context()
socket = context.socket(zmq.XPUB)
socket.connect("tcp://localhost:5555")
value = np.float32(18.69)
array = bytearray(value)
print("sending")
socket.send(array)
time.sleep(0.5)
print("sending")
socket.send(array)
time.sleep(0.5)
print("sending")
socket.send(b"PointData Finished")
time.sleep(0.5)
print("cutting connection")
socket.close()

