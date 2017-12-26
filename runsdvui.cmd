cd /d "C:\Users\Amr2\Desktop\DroneSimulation" &msbuild "DroneSimulation.csproj" /t:sdvViewer /p:configuration="Debug" /p:platform=Any CPU
exit %errorlevel% 