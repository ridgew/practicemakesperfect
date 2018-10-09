@echo off 
set num = 0 
For /r  . %%i in (*.mqh) do ( 
set /a num += 1 
echo %%i 
call echo 第 %%num%% 个文件处理成功 
ren "%%i" *.h)  
echo 共%num%个文件被处理成功 
pause>nul