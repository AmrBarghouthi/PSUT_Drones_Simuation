fileName  = 'comm.json'
file = fopen(fileName);
raw = fread(file,inf);
fclose(file);
str = char(raw');
data = jsondecode(str);




eps = 1;
window = 10;

lastEventTime =  max([data.logs(:).time]);
timeDomain = 0:eps:lastEventTime;

