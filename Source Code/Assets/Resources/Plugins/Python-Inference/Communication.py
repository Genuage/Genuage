import numpy as np
import random
import zmq
import time
import sys


class Socket:

	def __init__(self,address = "tcp://*:5555", data = []):
		self.address = address
		self.data = data
		self.context = zmq.Context()
		#self.socket = null
		#self.socket = self.context.socket(zmq.REP)
		#self.socket.bind(address)

	def createREPSocket(self):
		self.socket = self.context.socket(zmq.REP)
		self.socket.bind("tcp://127.0.0.1:5555")

	def createREQSocket(self):
		self.socket = self.context.socket(zmq.REQ)
		self.socket.connect("tcp://localhost:5555")
		
	def sendData(self,collumnData):
		self.createREPSocket()
		print ("starting sendData communication")
		index = 0
		while True:
			message = self.socket.recv()
			print(len(message))
			print(message)			
			if (index >= len(collumnData)):
				print ("transfer finished")
				self.socket.send(b"PointData Finished")
				break
			if (message == b"PointCollumn Echo") :
				print("message recognized")
				self.socket.send(collumnData[index])
				index+=1
		print("Closing connection")
		self.closeConnection()

	def receiveData(self):
		self.createREQSocket()
		print ("starting receiveData communication")
		resultByteArray = []
		while True:
			self.socket.send(b"PointCollumn Echo")
			time.sleep(0.01)
			#while True:
			message = self.socket.recv()
			#print(len(message))
			#print(message)
			if (message == b"PointData Finished"):
				print ("Transfer finished")
				self.closeConnection()
				return resultByteArray
			elif (len(message)> 0):
				print ("Column Received")
				resultByteArray.append(message)


	def closeConnection(self):
		print("cutting connection")
		#self.socket.close()
		self.context.destroy()

def ReceiveData():
	socket = Socket()
	array = socket.receiveData()
	#floatarray = convertBytesToFloats(array)
	return array

def transmitData(data_array):
	#Function you should call from another script
	socket = Socket()
	byte_array = convertDataToBytes(data_array)
	print("data conversion finished")
	socket.sendData(byte_array)

def convertBytesToFloats(byte_array):
	from sys import byteorder
	print(byteorder)
	# will print 'little' if little endian
	float_array = []
	newarray = np.array(byte_array)
	for i in range(len(byte_array)):
		float_array.append([])

	for j in range(len(byte_array)):

		float_array[j] = np.frombuffer(byte_array[j], dtype=np.float32)
		#assert np.array_equal(byte_array[j], float_array[j]), "Deserialization failed..."

	return float_array


def convertDataToBytes(data_array):
	byte_array = []
	newarray = np.array(data_array)
	for i in range(len(data_array)):
		byte_array.append([])

	for j in range(len(data_array)):

		byte_array[j] = np.float32(newarray[j]).tobytes()

	return byte_array
