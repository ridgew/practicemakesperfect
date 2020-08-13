@REM @CALL readini ITEM1 ITEM1_VALUE
@REM @ECHO %ITEM1_VALUE%

CALL inifile smom.ini "[MongoDB]" ITEM1
echo %ITEM1%