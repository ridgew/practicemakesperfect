:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:: 作者： namejm
:: http://bbs.bathome.net/thread-69-1-1.html
:: 出处： bbs.bathome.net
:: 编写日期： 2007-10-29～2008-10-12
:: 测试环境： CMD@WinXP SP2
:: 功能：按照exif信息归类照片
:: 详细介绍：
:: 　　本脚本通过获取jpg或jpeg照片的exif信息归类图片，具备按照分辨率大小和
:: 日期两种方式归类，并自动重命名同名文件，同时统计已经处理的照片数量。另外，
:: 可以选择使用如下功能：
:: 　　1、批量修正当前目录下及所有子目录中jpg或jpeg照片的修改日期为拍摄日期；
:: 　　2、能把所有子目录里的图片移动到父目录下并删除父目录下的所有空目录。
:: 注意：
:: 　　本脚本会读取照片的exif信息，在按日期整理照片时，自动修正照片的修改时间
:: 为照片的拍摄时间，时间精确到秒；没有exif信息的jpg或jpeg图片，修改时间保持
:: 不变，此时，按照像素大小整理照片的功能不可正常使用，按照日期归类照片时依据
:: 的是照片的最后修改日期。
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
@echo off
mode con lines=30
set TT=照片整理器1.4
color 1f

:source
title %TT%     当前目录："%cd%"
cls
echo                             ╭─────────╮
echo           ╭────────┤选择要整理的文件夹├─────────╮
echo           │                ╰─────────╯                  │
echo           │                                                        │
echo           │         本程序能把指定路径下的jpg格式照片按照分辨率    │
echo           │                                                        │
echo           │    大小或者拍摄日期整理到如下格式的文件夹中：          │
echo           │                                                        │
echo           │        1 分辨率大小 （如 800×600）                    │
echo           │                                                        │
echo           │    　　2 年-月-日（如 2007-01-01）                     │
echo           │                                                        │
echo           │        3 月\日期（如 01\2007-01-01）                   │
echo           │                                                        │
echo           │        4 年\日期（如 2007\2007-01-01）                 │
echo           │                                                        │
echo           │        5 年\月\日期 （如 2007\01\2007-01-01）          │
echo           │                                                        │
echo           │    　　注意：本脚本在按照日期归类照片时，会自动修正    │
echo           │                                                        │
echo           │    照片的创建时间为照片exif信息中的拍摄时间，精确到    │
echo           │                                                        │
echo           │    秒。没有exif信息的jpg图片，修改时间保持不变。       │
echo           │                                                        │
echo           ╰────────────────────────────╯
echo.
echo            处理当前目录请直接回车                          退出请按 0
echo           ──────────────────────────────
set Source=
set /p Source=              请把要处理的文件夹拖曳到本窗口：
if not defined Source set "Source=%cd%"
set "Source=%Source:"=%"
if "%Source%"=="0" exit
if not exist "%Source%\" goto source
echo "%Source%"|find ":">nul 2>nul||set "Source=%cd%\%Source%"
title %TT%     正在处理目录："%Source:"=%"

:fixtime
cls
echo                             ╭─────────╮
echo           ╭────────┤修正照片的修改日期├─────────╮
echo           │                ╰─────────╯                  │
echo           │                                                        │
echo           │         照片的拍摄时间包含在exif信息中，反映在原始     │
echo           │                                                        │
echo           │    照片文件中，就是照片的修改时间。                    │
echo           │                                                        │
echo           │    　　很不幸的是，照片的修改时间有时候并不是和exif    │
echo           │                                                        │
echo           │    信息中的拍摄时间完全吻合，比如：照片被编辑后、数    │
echo           │                                                        │
echo           │    据恢复后等等，从而导致在查看照片的修改时间时，难    │
echo           │                                                        │
echo           │    以判定照片的拍摄时间，在一些需要按照拍摄时间把资    │
echo           │                                                        │
echo           │    料归档的场合，这样的情形无疑是一种灾难。            │
echo           │                                                        │
echo           │    　　本功能可以把指定目录及其所有子目录下的照片修    │
echo           │                                                        │
echo           │    改时间重新设置为照片的拍摄时间——只要照片的exif    │
echo           │                                                        │
echo           │    信息没有被破坏掉。                                  │
echo           │                                                        │
echo           ╰────────────────────────────╯
echo.
echo            返回上一步请按 1                               退出请按 0
echo           ──────────────────────────────
set FixTime=
set /p FixTime=            是否修正照片的修改时间为拍摄时间（是=Y 否=其他任意键）：
if not defined FixTime goto movesub
set "FixTime=%FixTime:"=%"
if "%FixTime%"=="0" exit
if "%FixTime%"=="1" goto source
if /i "%FixTime%"=="y" (
    jhead -ft "%Source%\*.jp*g">nul 2>nul
    for /f "delims=" %%i in ('dir /ad /b /s "%Source%" 2^>nul') do (
        jhead -ft "%%i\*.jp*g">nul 2>nul
        title %TT%     正在处理目录："%%i"
    )
)

:movesub
cls
echo.&echo.&echo.
echo                             ╭─────────╮
echo           ╭────────┤   预处理子目录   ├─────────╮
echo           │                ╰─────────╯                  │
echo           │                                                        │
echo           │         如果当前目录的子目录中存在照片，而你需要把     │
echo           │                                                        │
echo           │    它们都放到父目录下，并删除父目录及其所有子目录下    │
echo           │                                                        │
echo           │    的空文件夹，那么，这个功能将是非常有用的。          │
echo           │                                                        │
echo           │    　　注意：此处的空文件夹是指删除父目录及其所有子    │
echo           │                                                        │
echo           │    目录下的Thumbs.db和*.ctg文件后形成的空文件夹，以    │
echo           │                                                        │
echo           │    及原本就存在的空文件夹。                            │
echo           │                                                        │
echo           ╰────────────────────────────╯
echo.
echo            返回上一步请按 1                                退出请按 0
echo           ──────────────────────────────
echo. 
set MoveSub=
set /p MoveSub=            是否把照片移动到父目录下并删除空目录（是=Y 否=N）：
if not defined MoveSub goto movesub
set "MoveSub=%MoveSub:"=%"
if not defined MoveSub goto movesub
if "%MoveSub%"=="0" exit
if "%MoveSub%"=="1" goto source
if /i "%MoveSub%"=="y" (
    set num=0
    set WhetherMove=M
    for /f "delims=" %%i in ('dir /ad /b /s "%Source%\" 2^>nul') do (
        for /f "delims=" %%j in ('dir /a-d /b /s "%%i\*.jp*g" 2^>nul') do (
            set route=&set postfix=
            setlocal enabledelayedexpansion
            call :re_name "%%j" "%Source%" "%%~nj" "%%~xj"
            move "%%j" "!Destination_1!">nul 2>nul
            endlocal
            set /a num+=1
        )
        title %TT%     正在处理文件："%%i"
    )
    del /a /f /q /s "%Source%\Thumbs.db">nul 2>nul
    del /a /f /q /s "%Source%\*.ctg">nul 2>nul
    for /f "delims=" %%i in ('dir /ad /b /s "%Source%\"^|sort /r') do (
        rd /q "%%i" 2>nul&title %TT%     处理文件："%%i"
    )
    title %TT%     正在处理的目录："%Source%"
    echo.&call echo                 共处理了 %%num%% 张照片，按任意键继续...
    pause>nul
) else if /i not "%MoveSub%"=="n" goto movesub

:whethermove
cls
echo.
echo                             ╭─────────╮
echo           ╭────────┤   照片处理方式   ├─────────╮
echo           │                ╰─────────╯                  │
echo           │                                                        │
echo           │         如果不想删除要处理文件夹下的照片，请选择复     │
echo           │                                                        │
echo           │    制功能；如果不保留原有的照片，请选择移动文件。      │
echo           │                                                        │
echo           │    　　注意：移动文件后，将会删除父目录下的所有空文    │
echo           │                                                        │
echo           │    件夹。此处的空文件夹是指删除父目录及其所有子目录    │
echo           │                                                        │
echo           │    下的Thumbs.db和*.ctg文件后形成的空文件夹，以及原    │
echo           │                                                        │
echo           │    本就存在的空文件夹。                                │
echo           │                                                        │
echo           │        另：同名文件将作重命名处理，具体方式为：在文    │
echo           │                                                        │
echo           │    件名后添加 －序号 。比如重命名为 abc-1.jpg          │
echo           │                                                        │
echo           ╰────────────────────────────╯
echo.
echo            返回上一步请按 1                                退出请按 0
echo           ──────────────────────────────
echo. 
set WhetherMove=
set /p WhetherMove=                   移动文件还是复制文件（移动=M  复制=C）：
if not defined WhetherMove goto whethermove
set "WhetherMove=%WhetherMove:"=%"
if "%WhetherMove%"=="0" exit
if "%WhetherMove%"=="1" goto movesub
set flag=
if /i "%WhetherMove%"=="M" set flag=1
if /i "%WhetherMove%"=="C" set flag=1
if not defined flag goto whethermove
if /i "%MoveSub%"=="y" goto destination

:whethersub
echo.
set WhetherSub=
set /p WhetherSub=                   是否处理子目录（是=Y 否=N）：
if not defined WhetherSub goto whethersub
set "WhetherSub=%WhetherSub:"=%"
if /i "%WhetherSub%"=="0" exit
if /i "%WhetherSub%"=="1" goto whethermove
if /i "%WhetherSub%"=="y" (
    set WhetherSub=/s
    goto destination
)
if /i "%WhetherSub%"=="n" (
    set WhetherSub=
    goto destination
)
goto whethersub

:destination
echo.
title %TT%     程序所在目录："%cd%"
set Destination=
set /p Destination=          请输入保存路径（保存在程序所在目录下请直接回车）：
if not defined Destination set "Destination=%cd%"
set "Destination=%Destination:"=%"
if "%Destination%"=="0" exit
if "%Destination%"=="1" goto whethermove
echo "%Destination%"|find ":">nul 2>nul||set "Destination=%cd%\%Destination%"
echo "%Destination%"|find /i "%Source%">nul 2>nul&&(
    cls
    for /l %%i in (1,1,7) do echo.
    echo　　　　　　为了不重复处理照片，不允许把保存路径设置到源文件所在目录下
    echo.&echo                            请重新设置保存路径
    echo.&echo.
    goto destination
)
md "%Destination%" 2>nul

:wise
cls
echo.&echo.&echo.&echo.&echo.
echo                               ╭────────╮
echo           ╭─────────┤  选择整理方式  ├─────────╮
echo           │                  ╰────────╯                  │
echo           │                                                        │
echo           │         有两种整理方式可供选择：                       │
echo           │                                                        │
echo           │         a  按文件尺寸整理 （如800×600）               │
echo           │                                                        │
echo           │         b  按照拍摄日期整理（如2008-01-01）            │
echo           │                                                        │
echo           ╰────────────────────────────╯
echo.
echo            返回上一步请按 1                                退出请按 0
echo           ──────────────────────────────
echo.
set Wise=
set /p Wise=                                 请选择（a/b）：
if not defined Wise goto wise
set "Wise=%Wise:"=%"
if not defined Wise goto wise
if "%Wise%"=="0" exit
if "%Wise%"=="1" goto whethermove
if /i "%Wise%"=="a" goto folders_dimesion
if /i "%Wise%"=="b" goto folders_date
goto wise

:folders_dimesion
cls
for /l %%i in (1,1,10) do echo.
echo                                正在处理中，请稍候...
set num=0
call :dimesion "%Source%"
if defined WhetherSub (
    for /f "delims=" %%i in ('dir /ad /b /s "%Source%" 2^>nul') do call :dimesion "%%i"
)
call :del_blank
pause>nul&goto source

:folders_date
cls
echo.&echo.&echo.
echo                             ╭─────────╮
echo           ╭────────┤  选择目录树格式  ├─────────╮
echo           │                ╰─────────╯                  │
echo           │                                                        │
echo           │         在保存路径下，将建立以照片拍摄日期为名的文     │
echo           │                                                        │
echo           │    件夹，这些文件夹的目录树结构如下：                  │
echo           │                                                        │
echo           │    　　a 年-月-日（如 2007-01-01）                     │
echo           │                                                        │
echo           │        b 月\日期（如 01\2007-01-01）                   │
echo           │                                                        │
echo           │        c 年\日期（如 2007\2007-01-01）                 │
echo           │                                                        │
echo           │        d 年\月\日期 （如 2007\01\2007-01-01）          │
echo           │                                                        │
echo           ╰────────────────────────────╯
echo.
echo            返回上一步请按 1                                退出请按 0
echo           ──────────────────────────────
echo.
set FoldersTree=
set /p  FoldersTree=                                 请选择（a/b/c/d）：
if not defined FoldersTree goto folders_date
set "FoldersTree=%FoldersTree:"=%"
if "%FoldersTree%"=="0" exit
if "%FoldersTree%"=="1" goto whethermove
set choice=
for %%i in (a b c) do if /i "%FoldersTree%"=="%%i" set choice=1
if not defined choice goto folders_date
cls
for /l %%i in (1,1,10) do echo.
echo                                正在处理中，请稍候...
set num=0
for /f "delims=" %%i in ('dir /a-d /b %WhetherSub% "%Source%\*.jp*g" 2^>nul') do (
    set /a num+=1
    jhead -ft "%%i">nul 2>nul
    if defined WhetherSub (
        for /f "delims=" %%j in ("%%i") do (
            set str=%%~tj
            set route=&set postfix=
            setlocal enabledelayedexpansion
            set str=!str:~0,10!
            set Y=!str:~0,4!&set M=!str:~5,2!&set D=!str:~8,2!
            if /i "%FoldersTree%"=="a" set Y=&set M=
            if /i "%FoldersTree%"=="b" set Y=
            if /i "%FoldersTree%"=="c" set M=
            md "%Destination%\!Y!\!M!\!str!" 2>nul
            if /i "%WhetherMove%"=="M" (
                call :re_name "%Source%\%%i" "%Destination%\!Y!\!M!\!str!" "%%~ni" "%%~xi"
                move "%%j" "!Destination_1!">nul 2>nul
            ) else (
                call :re_name "%Source%\%%i" "%Destination%\!Y!\!M!\!str!" "%%~ni" "%%~xi"
                copy "%%j" "!Destination_1!">nul 2>nul
            )
            title %TT%     正在处理文件："%%i"
            endlocal
        )
    ) else (
        for /f "delims=" %%j in ("%Source%\%%i") do (
            set str=%%~tj
            set route=&set postfix=
            setlocal enabledelayedexpansion
            set str=!str:~0,10!
            set Y=!str:~0,4!&set M=!str:~5,2!&set D=!str:~8,2!
            if /i "%FoldersTree%"=="a" set Y=&set M=
            if /i "%FoldersTree%"=="b" set Y=
            if /i "%FoldersTree%"=="c" set M=
            md "%Destination%\!Y!\!M!\!str!" 2>nul
            if /i "%WhetherMove%"=="M" (
                call :re_name "%Source%\%%i" "%Destination%\!Y!\!M!\!str!" "%%~ni" "%%~xi"
                move "%%j" "!Destination_1!">nul 2>nul
            ) else (
                call :re_name "%Source%\%%i" "%Destination%\!Y!\!M!\!str!" "%%~ni" "%%~xi"
                copy "%%j" "!Destination_1!">nul 2>nul
            )
            title %TT%     正在处理文件："%Source%\%%~nxi"
            endlocal
        )
    )
)
call :del_blank
pause>nul&goto source

:dimesion
:: 按照像素值在目的路径下建立文件夹
for /f "tokens=2,3*" %%i in ('jhead -cs nul "%~1\*.jp*g" 2^>nul^|findstr /i "name Resolution" 2^>nul') do (
    if "%%j"==":" (
        set "Source=%%k"&set "name=%%~nk"&set postfix=%%~xk
        title %TT%     正在处理文件："%%k"
    ) else (
        setlocal enabledelayedexpansion
        set "dimesion=%%j%%k"
        set "dimesion=!dimesion:x =×!"
        set "Destination=%Destination%\!dimesion!"
        md "!Destination!" 2>nul
        set route=&set postfix=
        if /i "%WhetherMove%"=="M" (
            call :re_name "!Source!" "!Destination!" "!name!" "!postfix!"
            move "!Source!" "!Destination_1!">nul 2>nul
        ) else (
            call :re_name "!Source!" "!Destination!" "!name!" "!postfix!"
            copy "!Source!" "!Destination_1!">nul 2>nul
        )
        endlocal
        set /a num+=1
    )
)
goto :eof

:re_name
if not defined route (
    set "route=%~2"
    set "filename=%~3"
    set postfix=%~4
)
:re_name_loop
if not exist "%route%\%filename%%-num%%postfix%" (
    set "Destination_1=%route%\%filename%%-num%%postfix%"
    set -num=&set _num=0
    goto :eof
) else (
    set /a _num+=1
    call set "-num=-%%_num%%"
    goto re_name_loop
)
goto :eof

:del_blank
if /i "%WhetherMove%"=="M" (
    del /a /f /q /s "%Source%\Thumbs.db">nul 2>nul
    del /a /f /q /s "%Source%\*.ctg">nul 2>nul
    for /f "delims=" %%i in ('dir /ad /b /s "%Source%\"^|sort /r') do (rd /q "%%i" 2>nul)
    rd /q "%Source%" 2>nul
)
title %TT%     当前目录："%cd%"
echo.&echo.&echo                        共处理了 %num% 张照片，按任意键继续...
goto :eof