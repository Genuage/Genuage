function TCPIPCommunicator(data)
    w = whos('data');
    disp(w);
    expectedmessage = 'PointCollumn Echo';
    pointnbr = size(data,2);
    columnnbr = size(data,1);
     
    if (pointnbr < columnnbr)
        data = data';
        pointnbr = size(data,2);
        columnnbr = size(data,1);
        disp('data had to be inverted');
    end
    disp(pointnbr);
    disp(columnnbr);
    tcpipSocket = tcpip('localhost',5555);
    tcpipSocket.Timeout = 50;
    tcpipSocket.ByteOrder = 'littleEndian';
    tcpipSocket = tcpip('localhost',5555);
    set(tcpipSocket, 'OutputBufferSize', (w.bytes));

    fopen(tcpipSocket);
    disp('starting connection');
    receivedmessage = fread(tcpipSocket,17,'char');
    disp(char(receivedmessage'));
    disp(expectedmessage);
    if strcmpi(char(receivedmessage'),expectedmessage)
        fwrite(tcpipSocket, 4*pointnbr,'int','sync')
        pause(0.5)
        for i = 1:columnnbr
            
            fwrite(tcpipSocket, data(i,:),'single','sync')
            disp('writing a collumn');
        end
            
    end
    
fclose(tcpipSocket);
end