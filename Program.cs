namespace ORM;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


class Program
{
    static async Task Main(string[] args)
    {
        using (var db = new EscuelaContext())
        {
            /* CREATE DATA BASE */
            db.Database.EnsureCreated(); // Crear la base de datos si no existe
            Console.WriteLine("Base de datos creada exitosamente!");

            /* DELETE REGISTERS */
            db.Alumnos.ExecuteDelete();
            db.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Alumnos';"); // ✅ Reinicia el AUTOINCREMENT
            Console.WriteLine("Base de datos limpiada exitosamente!");

            /* INSERT INTO - AGREGAR DATOS */
            db.Alumnos.Add(new Alumno { Nombre = "Juan", Edad = 10 });
            db.Alumnos.Add(new Alumno { Nombre = "Maria", Edad = 12 });

            // Agregar varios alumnos de una vez con AddRange()
            db.Alumnos.AddRange(
                new Alumno { Nombre = "Jose", Edad = 10 },
                new Alumno { Nombre = "Ana", Edad = 18 },
                new Alumno { Nombre = "Isabel", Edad = 16 },
                new Alumno { Nombre = "Rubén", Edad = 19 },
                new Alumno { Nombre = "Iñigo", Edad = 16 },
                new Alumno { Nombre = "Sergio", Edad = 20 }
            );

            db.SaveChanges(); // Guardamos cambios

            // con AWAIT
            await db.Alumnos.AddAsync(new Alumno { Nombre = "Pepe", Edad = 14 });
            await db.SaveChangesAsync();
            Console.WriteLine("\nAlumnos agregados exitosamente!");

            /* SELECT */
            var alumnosList = db.Alumnos.ToList(); // Recuperamos todos los alumnos - aplicaciones PEQUEÑAS

            // Con AWAIT
            var alumnosListAsync = await db.Alumnos.ToListAsync(); // NO BLOQUEAMOS PROGRAMA - APLICACIONES GRANDES

            foreach (var alumno in alumnosList)
            {
                Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
            }

            /* WHERE */
            Console.WriteLine("\nAlumnos mayores de 18:");

            var alumnosMayores = db.Alumnos.Where(alumno => alumno.Edad >= 18).ToList();

            foreach (var alumno in alumnosMayores)
            {
                Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
            }

            /* DELETE */
            Console.WriteLine("\nBorramos a Jose");

            var alumnoJose = db.Alumnos.FirstOrDefault(alumno => alumno.Nombre == "Jose");

            if (alumnoJose != null)
            {
                Console.WriteLine($"ID: {alumnoJose.Id}, Nombre: {alumnoJose.Nombre}, Edad: {alumnoJose.Edad}\n");
                db.Alumnos.Remove(alumnoJose);
                db.SaveChanges();
            }

            alumnosList = db.Alumnos.ToList();

            foreach (var alumno in alumnosList)
            {
                Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
            }

            Console.WriteLine("\nBorramos todos los alumnos con menos de 16 años.");

            // Forma "antigua"
            // List<Alumno> alumnosMenores = db.Alumnos.Where(alumno => alumno.Edad < 16).ToList();

            // db.Alumnos.RemoveRange(alumnosMenores);
            // db.SaveChanges();

            // Forma nueva
            db.Alumnos.Where(alumno => alumno.Edad < 16).ExecuteDelete(); // no necesita db.SaveChanges

            alumnosList = db.Alumnos.ToList();

            foreach (Alumno alumno in alumnosList)
            {
                Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
            }
            ;

            /* UPDATE */
            Console.WriteLine("Aumentando la edad de todos los alumnos...");

            // ✅ Método optimizado en EF Core 7+
            db.Alumnos.ExecuteUpdate(setters =>
                setters.SetProperty(a => a.Edad, a => a.Edad + 1));

            Console.WriteLine("Edades actualizadas. Lista de alumnos:");

            // Limpiamos caché a ver si suma bien
            db.ChangeTracker.Clear(); // ✅ Limpia la caché del DbContext. Después de ExecuteUpdate() o ExecuteDelete() → Para asegurarte de que los datos reflejan los cambios.

            // Mostrar alumnos actualizados
            alumnosList = db.Alumnos.ToList();
            foreach (var alumno in alumnosList)
            {
                Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
            }
        }
    }
}
