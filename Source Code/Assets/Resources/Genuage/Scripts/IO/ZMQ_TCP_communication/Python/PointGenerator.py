import numpy as np
import matplotlib.pyplot as plt
import random
import zmq
import time


def f(x, y):
    return np.sin(np.sqrt(x ** 2 + y ** 2))

print ("how many points do you want to create ?")
pointnb = int(input())




array = []
byte_array = []
for h in range(5):
	array.append([])
	byte_array.append([])


for n in range(pointnb):
	radius =  50
	x = random.gauss(pointnb, pointnb/2)
	y = random.gauss(pointnb, pointnb/2)
	z = random.gauss(pointnb, pointnb/2)
	norm = 1/np.sqrt(x**2+y**2+z**2)
	if (x!=0 and y!=0 and z!=0):
		x = x*norm
		y = y*norm
		z = z*norm
		x = np.float32(x*radius)
		y = np.float32(y*radius)
		z = np.float32(z*radius)

		array[0].append(x)
		array[1].append(y)
		array[2].append(z)
		array[3].append(np.float32(np.random.uniform(0.001,10000.0)))
		array[4].append(np.float32(np.random.uniform(0.001,10000.0)))
narray = np.array(array)

for k in range(len(byte_array)):
	byte_array[k] = narray[k].tobytes()

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")
print ("starting communication")
index = 0
while True:
	message = socket.recv()
	if (index >= len(byte_array)):
		print ("transfer finished")
		socket.send(b"PointData Finished")
		break

	if (message == b"PointCollumn Echo") : 
		print("message recognized")
		socket.send(byte_array[index])
		index+=1

print("cutting connection")
socket.close()