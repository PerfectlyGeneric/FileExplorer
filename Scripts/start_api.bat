@echo off
setlocal

set "PORT=8000"
set "HOST=0.0.0.0"

netstat -an | findstr /R /C:":%PORT% .*LISTENING" >nul
if %errorlevel%==0 (
    echo [INFO] API is already running on port %PORT%.
    exit /b 0
)

cd /d "E:\Proj_enter\FileExplorer\API"

title AI Model API - Port %PORT%

echo [INFO] Starting Python API...

py -m uvicorn Python_ai_API:app --host %HOST% --port %PORT% --log-level info

if %errorlevel% neq 0 (
    echo [ERROR] The API crashed or failed to start.
    pause
)