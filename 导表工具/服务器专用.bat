@echo off
set curPath=%cd%
set CodePath=%curPath%\..\ExportFile\CSV
set CodeExportPath=%curPath%\..\Tools\Tools\CSV\CSV


echo %curPath%
echo delete file
for /r %CodeExportPath% %%i in (*) do (  
	del /a /f /q "%%i"
)  

echo Copy%CodePath%
echo To%CodeExportPath%
for /r %CodePath% %%i in (*) do (  
echo "%%~fi"  
copy /y  "%%~fi"  %CodeExportPath%
) 


exit