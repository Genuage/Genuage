import numpy as np
import matplotlib.pyplot as plt
import random
linalg = np.linalg

stress_count = [10000, 100000, 300000, 500000, 1000000]
def f(x, y):
    return np.sin(np.sqrt(x ** 2 + y ** 2))

for n in stress_count:
	a = open("read_"+str(n)+".3d", "w")
	radius =  50
	for p in range(n):
		x = random.gauss(n, n/2)
		y = random.gauss(n, n/2)
		z = random.gauss(n, n/2)
		norm = 1/np.sqrt(x**2+y**2+z**2)
		if (x!=0 and y!=0 and z!=0):
			x = x*norm
			y = y*norm
			z = z*norm
			x= x*radius
			y= y*radius
			z= z*radius
			a.write(str(x)+ "\t" + str(y) + "\t" + str(z) + "\t1" +"\t1" + "\n")
	a.close()

