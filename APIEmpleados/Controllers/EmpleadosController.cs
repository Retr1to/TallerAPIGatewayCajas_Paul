using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIEmpleados.Data;
using APIEmpleados.Models;

namespace APIEmpleados.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    private readonly EmpleadosContext _db;

    public EmpleadosController(EmpleadosContext context)
    {
        _db = context;
    }

    // GET: api/empleados
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
    {
        return await _db.Empleados.ToListAsync();
    }

    // GET: api/empleados/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Empleado>> GetEmpleado(int id)
    {
        var empleado = await _db.Empleados.FindAsync(id);

        if (empleado == null)
        {
            return NotFound();
        }

        return empleado;
    }

    // POST: api/empleados
    [HttpPost]
    public async Task<ActionResult<Empleado>> PostEmpleado(Empleado empleado)
    {
        // Convertir DateTime a UTC si no lo está
        empleado.FechaContratacion = NormalizeToUtc(empleado.FechaContratacion);

        _db.Empleados.Add(empleado);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado);
    }

    // PUT: api/empleados/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmpleado(int id, Empleado empleado)
    {
        if (id != empleado.Id)
        {
            return BadRequest();
        }

        // Convertir DateTime a UTC si no lo está
        empleado.FechaContratacion = NormalizeToUtc(empleado.FechaContratacion);

        _db.Entry(empleado).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmpleadoExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/empleados/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmpleado(int id)
    {
        var empleado = await _db.Empleados.FindAsync(id);
        if (empleado == null)
        {
            return NotFound();
        }

        _db.Empleados.Remove(empleado);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private bool EmpleadoExists(int id)
    {
        return _db.Empleados.Any(e => e.Id == id);
    }

    private static DateTime NormalizeToUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Unspecified)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        if (value.Kind == DateTimeKind.Local)
        {
            return value.ToUniversalTime();
        }

        return value;
    }
}
