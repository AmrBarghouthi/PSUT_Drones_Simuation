Table = readtable('test.txt');

d = table2array(Table);

t = d(:,1);
x = d(:,2);
y = d(:,3);
z = d(:,4);
plot(t,x);
xlabel('time');
ylabel('x cordinate');
figure
plot(t,y);
xlabel('time');
ylabel('y cordinate');
figure
plot(t,z);
xlabel('time');
ylabel('z cordinate');
