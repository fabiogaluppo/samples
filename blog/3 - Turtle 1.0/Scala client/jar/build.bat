@ECHO OFF

IF EXIST turtle.jar DEL turtle.jar

CD bin

%JDK_HOME%\bin\jar -cfm ..\turtle.jar ..\MANIFEST.MF *.*

CD ..