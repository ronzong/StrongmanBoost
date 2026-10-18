@echo off
setlocal
chcp 65001 >nul
set "PYTHONUTF8=1"
where python >nul 2>nul
if not errorlevel 1 (
    python "%~dp0Tools\repackage-facebook.py" --interactive
    goto result
)
where py >nul 2>nul
if not errorlevel 1 (
    py -3 "%~dp0Tools\repackage-facebook.py" --interactive
    goto result
)
echo Python 3 is required. Please install Python and try again.
pause
exit /b 1

:result
set "fbResult=%errorlevel%"
if not "%fbResult%"=="0" echo Repackaging failed or was cancelled. See the message above.
echo.
pause
exit /b %fbResult%
