# Taller Avanzadas API

Este proyecto contiene dos APIs en .NET:

## Proyectos

### 1. APIEmpleados
API para gestión de empleados con operaciones CRUD.

**Puerto:** http://localhost:5001

**Endpoints:**
- GET /api/empleados - Obtener todos los empleados
- GET /api/empleados/{id} - Obtener empleado por ID
- POST /api/empleados - Crear nuevo empleado
- PUT /api/empleados/{id} - Actualizar empleado
- DELETE /api/empleados/{id} - Eliminar empleado

**Base de datos:** PostgreSQL
- Host: localhost
- Database: empleadosdb
- Username: postgres
- Password: postgres

### 2. APIGateway
Gateway con autenticación JWT usando Ocelot.

**Puerto:** http://localhost:5000

**Endpoints:**
- POST /api/auth/login - Autenticación (devuelve JWT)
- GET /empleados - Proxy a APIEmpleados (requiere JWT)
- GET /empleados/{id} - Proxy a APIEmpleados (requiere JWT)
- POST /empleados - Proxy a APIEmpleados (requiere JWT)
- PUT /empleados/{id} - Proxy a APIEmpleados (requiere JWT)
- DELETE /empleados/{id} - Proxy a APIEmpleados (requiere JWT)

## Configuración

### Requisitos previos
- .NET 9.0 SDK
- PostgreSQL instalado y ejecutándose

### Pasos para ejecutar

1. **Crear la base de datos:**
   ```bash
   # Aplicar migraciones
   dotnet ef database update --project APIEmpleados
   ```

2. **Ejecutar APIEmpleados:**
   ```bash
   dotnet run --project APIEmpleados
   ```

3. **Ejecutar APIGateway (en otra terminal):**
   ```bash
   dotnet run --project APIGateway
   ```

## Uso

### 1. Obtener token de autenticación:
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

Respuesta:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2026-01-21T20:00:00Z"
}
```

### 2. Usar el token para acceder a los empleados:
```bash
curl -X GET http://localhost:5000/empleados \
  -H "Authorization: Bearer {token}"
```

### 3. Crear un empleado:
```bash
curl -X POST http://localhost:5000/empleados \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Juan",
    "apellido": "Pérez",
    "email": "juan.perez@example.com",
    "cargo": "Desarrollador",
    "salario": 50000,
    "fechaContratacion": "2024-01-15"
  }'
```

## Credenciales de prueba

**Login:**
- Username: admin
- Password: admin123

**PostgreSQL:**
- Username: postgres
- Password: postgres

## Configuración JWT

La clave secreta y configuración JWT se encuentra en los archivos `appsettings.json` de ambos proyectos.

**Importante:** En producción, almacenar la clave JWT en variables de entorno o Azure Key Vault.
