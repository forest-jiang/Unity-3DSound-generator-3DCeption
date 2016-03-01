import socket
#import encode
import base64
from io import BytesIO
import numpy as np
import time

def sendData(so, string):
   so.send(string) 
   data = ''
   print "waiting"
   data = so.recv(1024) #.decode()
   return string==data[:len(string)]
	
def connectAndSend(host, port, st):
	s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
	host = host
	port = port
	print "sending",st
	s.connect((host,port))
	data=sendData(s, st)
	s.close ()
	print "got reply, consistency check: ", data
	return data

if __name__ == "__main__":
	time.sleep(0.3)
	connectAndSend('127.0.0.1', 5555, '0,0,1,0')
	time.sleep(1)
	connectAndSend('127.0.0.1', 5555, '-1,0,1,0\n1,0,1,1')
	#print "Now, test mode, some weird string"
	#time.sleep(1)
	#connectAndSend('127.0.0.1', 5555, "\n")
	#time.sleep(1)
	#connectAndSend('127.0.0.1', 5555, "") # could not be empty, should not have \n in the end
	
	