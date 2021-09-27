%function to calculate the apparent diffusion coefficient of
%single-particle data 

%inputs: 
%(i) tracks : contains the list of localizations of the track of interrset
% in tracks the column should be organized as follows X, Y, Z, I, Frame
% number
%(ii) f_position : conversion factor to change the positions to nanometers
%(ii) f_time : conversion factor to change the format to frame number
%(ii) dt : time interval between consecutive frames in ms
%(iii) c : dimensionality factor  2 for 1D, 4 for 2D, 6 for 3D

%output: 
% D_coef : diffusion coefficient in um2/s

function D_coef=DiffusionCoef(tracks, f_position, f_time, dt, c)

tracks(:,1:3) = round(tracks(:,1:3)/f_position);

tracks(:,5) = round(tracks(:,5)/f_time);
MSD = [];
dr2=0;

    delta_t =1; %starting delta
    delta_t_max=floor((tracks(end,5)-tracks(1,5))-5);% to be able to average at least 5 
    if delta_t_max>delta_t
            [m,n]=size(tracks);

            %MSD{i}(:,:) =[];
            A=[];
            for delta_t=1:delta_t_max
                k=0;

                for j=1:m % over all the time points of the track
                    if tracks(j,5)+delta_t<tracks(end,5)
                        ind=find(tracks(:,5)==tracks(j,5)+delta_t);
                            if tracks(ind,5)==tracks(j,5)+delta_t
                                tempA = tracks(j,1);
                                tempB = tracks(ind(1),1);
                                tempC = tracks(j,2);
                                tempD = tracks(ind(1),2);
                                tempE = tracks(j,3);
                                tempF = tracks(ind(1),3);
                                
                                tempG = ((tempA-tempB)^2)+((tempC-tempD)^2)+((tempE-tempF)^2);
                                dr2=dr2+tempG; %
                                k=k+1;
                            end
                    end


                end
                if k~=0
                    A=[A;delta_t , dr2/k];
                end
                %MSD{i}(:,:) = [ MSD{i}(:,:) ;  ]; %all in nm^2
                dr2=0;
            end

            MSD=A;%all in nm^2
    
    end



% Calculate Diffusion coefficient over the first 5xdt
    %fit with linear function between 2dt and 5or7dt (there is a paper speaking about that 1991)
    D_start=2;
    D_end=5;
    
    ind_start=find(MSD(:,1)==D_start);
    ind_end=find(MSD(:,1)==D_end);
    
    delta_t=MSD(ind_start(1):ind_end(1),1);
    y_MSD=MSD(ind_start(1):ind_end(1),2);
    
    %D_coef = 0.0;
    %everything is still in nm^2             
                %[a,b]=fit(delta_t,y_MSD,'poly1');
                a = polyfit(delta_t,y_MSD,1);
                %Y=a.p1*delta_t+a.p2;
    %calculate Diffusion coefficient from the slope and change unit to um^2/s
    %D_coef=10^-6*a.p1/(c*dt*10^-3); 
    D_coef=double(10^-6*a(1)/(c*dt*10^-3)); 

    if D_coef<0 
        D_coef=0;
    end




end