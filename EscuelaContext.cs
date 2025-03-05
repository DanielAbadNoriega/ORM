using Microsoft.EntityFrameworkCore;

class EscuelaContext : DbContext
{
    public DbSet<Alumno> Alumnos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=escuela.db");
}