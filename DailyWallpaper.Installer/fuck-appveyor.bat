REM cd to project dir.
@echo off
echo I should in project diretory
ECHO COULD dir
RMDIR /Q /S "..\OUTPUT"
md "..\OUTPUT"
copy "..\DailyWallpaper\%1"  "..\OUTPUT" /y
del "..\OUTPUT\DailyWallpaper.config*" 
del "..\OUTPUT\DailyWallpaper.pdb" 
del "..\OUTPUT\*.log.txt"
echo fuck appveyor: NO XCOPY.
set errorlevel=0