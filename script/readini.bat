@CALL :readini %1 %2

:readini
  @for /f "skip=1 tokens=1,2 delims==" %%a IN (smom.ini) Do @if %1==%%a set %2=%%b
  @goto :eof