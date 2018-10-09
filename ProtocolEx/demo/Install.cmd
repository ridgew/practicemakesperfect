call "c:\Program Files\Microsoft Visual Studio .NET 2003\Common7\Tools\vsvars32.bat"

gacutil /i protocol.dll
regasm protocol.dll
