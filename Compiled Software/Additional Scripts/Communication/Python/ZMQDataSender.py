import numpy as np
import matplotlib.pyplot as plt
import random
import zmq
import time
import sys


class Socket:

	def __init__(self,address = "tcp://*:5555", data = []):
		self.data = data
		self.context = zmq.Context()
		self.socket = self.context.socket(zmq.REP)
		self.socket.bind(address)

	def sendData(self,collumnData):
		print ("starting communication")
		index = 0
		while True:
			message = self.socket.recv()
			if (index >= len(collumnData)):
				print ("transfer finished")
				self.socket.send(b"PointData Finished")
				break

			if (message == b"PointCollumn Echo") : 
				print("message recognized")
				self.socket.send(collumnData[index])
				index+=1
		
		self.closeConnection()


	def closeConnection(self):
		print("cutting connection")
		self.socket.close()

def RandomGeneratorGaussian(pointnumber):
    array = []
    for h in range(7):
        array.append([])
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
            array[5].append(np.float32(np.random.uniform(0.001,10000.0)))
            array[6].append(np.float32(np.random.uniform(0.001,10000.0)))
    return array


def convertDataToBytes(data_array):
	byte_array = []
	newarray = np.array(data_array)
	for i in range(len(data_array)):
		byte_array.append([])
	
	for j in range(len(data_array)):
		
		byte_array[j] = np.float32(newarray[j]).tobytes()
	
	return byte_array

def transmitData(data_array):
	#Main function, will create a socket and send the data array supplied in the parameter, this is the function you should call from another script 
	socket = Socket()
	byte_array = convertDataToBytes(data_array)
	socket.sendData(byte_array)

def transmitDatafrom1DArray(array1d,rownbr,collumnnbr):
    newarray = []
    index = 0
    for i in range(collumnnbr):
        newarray.append([])
        for j in range(rownbr):
            newarray[i].append(array1d[index])
            index+=1
    transmitData(newarray)

if __name__ == "__main__" :

    print ("Point Size set to 20 000 by default")
    pointnb = 20000
    
    array = RandomGeneratorGaussian(pointnb)
    transmitData(array)