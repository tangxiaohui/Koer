@echo off
set curPath=%cd%
set CodePath=%curPath%\..\ExportFile\ClientCode
set CodeExportPath=%curPath%\..\Client\Assets\Code\Generate

set TextCode=%curPath%\..\ExportFile\ClientLanguage\TEXT.cs
set TextLang=%curPath%\..\ExportFile\ClientLanguage\TEXT.lang
set TextLua=%curPath%\..\ExportFile\ClientLanguage\TEXT.lua

set TextExportCode=%curPath%\..\Client\Assets\Code\Generate
set TextExportLang=%curPath%\..\Client\Assets\StreamingAssets\Texts
set TextExportLua=%curPath%\..\Client\Assets\Lua\Common

set BinPath=%curPath%\..\ExportFile\Bin
set BinExportPath=%curPath%\..\Client\Assets\StreamingAssets\GenerateData


 if exist %BinExportPath% (        
        echo 目录%BinExportPath%已存在，无需创建  
    ) else (  
        echo 创建%BinExportPath=%         
        md %BinExportPath%  
    )  

 if exist %TextExportLang% (        
        echo 目录%TextExportLang%已存在，无需创建  
    ) else (  
        echo 创建%TextExportLang%         
        md %TextExportLang%  
    )  


echo %curPath%
echo delete file
for /r %BinExportPath% %%i in (*) do ( 
	del /a /f /q "%%i"
)

for /r %CodeExportPath% %%i in (*) do (  
	del /a /f /q "%%i"
)  

for /r %TextExportCode% %%i in (*) do (  
	del /a /f /q "%%i"
)  

for /r %TextExportLang% %%i in (*) do (  
	del /a /f /q "%%i"
)  

del /a /f /q %TextExportLua%\TEXT.lua


echo Copy%CodePath%
echo To%CodeExportPath%
for /r %CodePath% %%i in (*) do (  
echo "%%~fi"  
copy /y  "%%~fi"  %CodeExportPath%
) 



echo Copy%BinPath%
echo To%BinExportPath%
for /r %BinPath% %%i in (*) do (  
echo "%%~fi"  
copy /y  "%%~fi"  %BinExportPath%
) 




echo %TextCode% 
copy /y %TextCode%  %TextExportCode%
echo %TextLang% 
copy /y %TextLang%  %TextExportLang%
echo %TextLua%
copy /y %TextLua%  %TextExportLua%


exit