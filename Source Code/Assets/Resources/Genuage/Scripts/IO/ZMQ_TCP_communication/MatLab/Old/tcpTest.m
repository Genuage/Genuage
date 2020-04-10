expectedmessage = 'PointCollumn Echo'     ;

collumnnbr = 1;
pointnbr = 10;
%array = generateArray(pointnbr,collumnnbr,0.01,100.01);
array = (1:pointnbr);
whos array;
w = whos ('array');
%disp(w.bytes);
%disp(array);
tcpipSocket = tcpip('localhost',5555);
tcpipSocket.Timeout = 50;
%tcpipSocket.ByteOrder = 'littleEndian';
tcpipSocket = tcpip('localhost',5555);
set(tcpipSocket, 'OutputBufferSize', (w.bytes));

fopen(tcpipSocket);
disp('starting connection');
i = 1;
%while true    

    %pause(2);
    receivedmessage = fread(tcpipSocket,17,'char');
    disp(char(receivedmessage'));
    disp(expectedmessage);
    if strcmpi(char(receivedmessage'),expectedmessage)
        %(tcpipSocket, 4*pointnbr,'int','sync')
        for i = 1:collumnnbr
            disp('writing a collumn');
            fwrite(tcpipSocket, array(i,:),'int','sync')
            %pause(1)
        end
        %pause(10);
        %i = i+1;
    end
    
    
    


fclose(tcpipSocket);


function ZMQCommunicator(data)
    pointnbr = size(data,1);
    collumnnbr = size(data,2);
    tcpipSocket = tcpip('localhost',5555);
    tcpipSocket.Timeout = 50;
    %tcpipSocket.ByteOrder = 'littleEndian';
    tcpipSocket = tcpip('localhost',5555);
    set(tcpipSocket, 'OutputBufferSize', (w.bytes));

    fopen(tcpipSocket);
    disp('starting connection');
    i = 1;
    receivedmessage = fread(tcpipSocket,17,'char');
    disp(char(receivedmessage'));
    disp(expectedmessage);
    if strcmpi(char(receivedmessage'),expectedmessage)
        fwrite(tcpipSocket, 4*pointnbr,'int','sync')
        pause(2)
        for i = 1:collumnnbr
            disp('writing a collumn');
            fwrite(tcpipSocket, data(i,:),'single','sync')
        end
            %pause(10);
            %i = i+1;
    end
    
    
    


fclose(tcpipSocket);
end





function array = generateArray(pointnbr, collumnnbr, min, max)
    array = zeros(collumnnbr,pointnbr);
    for i = 1:collumnnbr
        for j = 1:pointnbr
            array(i,j) = single(rand*(max-min));
        end
    end
end