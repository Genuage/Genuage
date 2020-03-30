clear all
close all

c = load('FILEPATH');

b=single(c);

TCPIPCommunicator(b)