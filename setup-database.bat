@echo off
echo ========================================
echo Configurando Base de Datos PostgreSQL
echo ========================================
echo.

echo Aplicando migraciones a la base de datos...
cd APIEmpleados
dotnet ef database update

echo.
if %ERRORLEVEL% EQU 0 (
    echo ========================================
    echo Base de datos configurada exitosamente!
    echo ========================================
) else (
    echo ========================================
    echo Error al configurar la base de datos
    echo ========================================
    echo.
    echo Asegurate de que:
    echo 1. PostgreSQL este instalado y ejecutandose
    echo 2. Las credenciales en appsettings.json sean correctas
    echo 3. El usuario postgres tenga permisos adecuados
)

echo.
cd ..
pause
