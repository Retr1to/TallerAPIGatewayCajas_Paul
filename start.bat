@echo off
echo Iniciando APIEmpleados en puerto 5001...
start cmd /k "cd APIEmpleados && dotnet run"

timeout /t 3 /nobreak >nul

echo Iniciando APIGateway en puerto 5000...
start cmd /k "cd APIGateway && dotnet run"

echo.
echo ========================================
echo Ambos proyectos se están iniciando...
echo ========================================
echo APIEmpleados: http://localhost:5001
echo APIGateway: http://localhost:5000
echo ========================================
echo.
echo Para probar la autenticación:
echo POST http://localhost:5000/api/auth/login
echo Body: {"username":"admin","password":"admin123"}
echo.
pause
