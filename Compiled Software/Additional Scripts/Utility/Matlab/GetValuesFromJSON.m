function GetValuesFromJSON(path)
    text = fileread(path);
    FullData = jsondecode(text);
    
    HistogramValues = FullData.histogramsaveList;
    
    
end