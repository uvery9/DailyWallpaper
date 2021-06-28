REM cd to project dir.
REM FUCK
echo "bin\Release\"
RM /Q /S "..\OUTPUT"
md "..\OUTPUT"
copy "bin\Release\"  "..\OUTPUT" /y
del "..\OUTPUT\config*.ini" 
del "..\OUTPUT\*.log.txt"
echo fuck appveyor.
set errorlevel=0