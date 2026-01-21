using Microsoft.EntityFrameworkCore;
using APIEmpleados.Models;

namespace APIEmpleados.Data;

public class EmpleadosContext : DbContext
{
    private const int NombreMaxLength = 100;
    private const int ApellidoMaxLength = 100;
    private const int EmailMaxLength = 200;
    private const int CargoMaxLength = 100;

    public EmpleadosContext(DbContextOptions<EmpleadosContext> options)
        : base(options)
    {
    }

    public DbSet<Empleado> Empleados { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Empleado>(ConfigureEmpleado);
    }

    private static void ConfigureEmpleado(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Empleado> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Nombre).IsRequired().HasMaxLength(NombreMaxLength);
        entity.Property(e => e.Apellido).IsRequired().HasMaxLength(ApellidoMaxLength);
        entity.Property(e => e.Email).IsRequired().HasMaxLength(EmailMaxLength);
        entity.Property(e => e.Cargo).IsRequired().HasMaxLength(CargoMaxLength);
        entity.Property(e => e.Salario).HasColumnType("decimal(18,2)");
    }
}
