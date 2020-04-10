collumnnbr = 5;
pointnbr = 100000;
array = generateArray(pointnbr,collumnnbr,0.01,100.01);
whos array;
sendData(array,collumnnbr);

%echotcpip('off')

function sendData(data,collumnnbr)
    tcpipSocket = tcpip('localhost',5555);
    tcpipSocket.ByteOrder = 'littleEndian';
    w = whos ('data');
    set(tcpipSocket, 'OutputBufferSize', (w.bytes));
    fopen(tcpipSocket);
    disp('starting connection');
    disp(tcpipSocket.OutputBufferSize);
    for i = 1:collumnnbr

        fwrite(tcpipSocket,data(i,:),'single','sync');
        disp('message sent');
        pause(2);
               pause(2);
 fwrite(tcpipSocket,'Collumn Finished','char','sync');

    end
    pause(1);

    fwrite(tcpipSocket,'PointData Finished','char','sync');
    disp('Finished');
    pause(1);
    fclose(tcpipSocket);
    disp('Connection cut');
    echotcpip('off')
end


