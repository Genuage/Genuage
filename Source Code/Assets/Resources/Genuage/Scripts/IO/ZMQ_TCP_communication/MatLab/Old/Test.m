

message = [single(65.59) single(87.4) single(17.4);single(452.04) single(4642.0014) single(65.59); single(65.59) single(65.59) single(65.59)];
index = 0;
echotcpip('off')
echotcpip('on',5555)
tcpipSocket = tcpip('localhost',5555);
fopen(tcpipSocket);
disp('starting connection');
while true
    

    response = fread(tcpipSocket,17,'char');

    if index >= size(message)
        newmessage = 'PointData Finished';
        set(tcpipSocket, 'OutputBufferSize', newmessage.bytes);
        fwrite(tcpipSocket,newmessage,'char');
        break;
    end
    if strcmpi(response, 'PointCollumn Echo')
        disp('message recieved');
        set(tcpipSocket, 'OutputBufferSize', message(index).bytes);
        fwrite(tcpipSocket,message(index),'float32');
        index = index+1;
    end

    
end
fclose(tcpipSocket);
echotcpip('off')






