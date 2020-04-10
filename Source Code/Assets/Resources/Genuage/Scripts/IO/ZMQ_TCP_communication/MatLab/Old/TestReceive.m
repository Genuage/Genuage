message = [single(65.59) single(87.4) single(17.4);single(452.04) single(4642.0014) single(65.59); single(65.59) single(65.59) single(65.59)];
index = 0;
%echotcpip('off')
%echotcpip('on',5555)
tcpipSocket = tcpip('localhost',5555);
fopen(tcpipSocket);
disp('starting connection');
while true
    

    response = fread(tcpipSocket,17,'char');
    disp(response);
    
end

    fclose(tcpipSocket);
