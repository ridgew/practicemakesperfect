@rem https://blog.csdn.net/zhanglu_1024/article/details/103436945
@rem bat文件设置自动关机以及弹框确认
@echo off
setlocal enabledelayedexpansion
 
set Vbscript=Msgbox("你的电脑将在五分钟后关机，是否继续？",1,"系统提醒")
for /f "Delims=" %%a in ('MsHta VBScript:Execute("CreateObject(""Scripting.Filesystemobject"").GetStandardStream(1).Write(%Vbscript:"=""%)"^)(Close^)') do Set "MsHtaReturnValue=%%a"
set ReturnValue1=同意关机
set ReturnValue2=取消操作
echo 你点击了!ReturnValue%MsHtaReturnValue%!
if %MsHtaReturnValue% == 1 (
    echo 关机啦！！！
	shutdown -s -t 300
) else (
    echo 取消成功！！！
)
 
pause