[fileName fileDir]  = uigetfile({'*.json'},'File Selector')
path = [fileDir fileName]
file = fopen(path);
raw = fread(file,inf);
fclose(file);
str = char(raw');
data = jsondecode(str);





 

