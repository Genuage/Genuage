function array = GenerateArray(pointnbr, collumnnbr, min, max)
    array = zeros(collumnnbr,pointnbr);
    for i = 1:collumnnbr
        for j = 1:pointnbr
            array(i,j) = single(rand*(max-min));
        end
    end
end