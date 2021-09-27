@echo off

call "setup_msvc.bat"

cd .

chcp 1252

if "%1"=="" (nmake  -f DiffusionCoef_rtw.mk all) else (nmake  -f DiffusionCoef_rtw.mk %1)
@if errorlevel 1 goto error_exit

exit /B 0

:error_exit
echo The make command returned an error of %errorlevel%
exit /B 1