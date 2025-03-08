# ORM

## 1. Creamos una clase Alumno

```C#
class Alumno {
    public int Id { get; set; } // PRIMARY KEY
    public string Nombre { get; set; }
    public int Edad { get; set; }
}
```

> Esto es lo que nos servirá como plantilla de la tabla

## 2. Contexto de la base de datos

### **1️⃣ Abrir la Consola de Administrador de Paquetes (Package Manager Console)**
**Este método es más fácil si usas Visual Studio en modo gráfico.**

✅ **Pasos:**
1. Abre **Visual Studio (Microsoft)**.
2. Ve al menú **"Herramientas"** (`Tools`).
3. Selecciona **"Administrador de paquetes NuGet"** → **"Consola del Administrador de paquetes"**.
4. **En la consola**, escribe:
   ```powershell
   Install-Package Microsoft.EntityFrameworkCore.Sqlite
   ```
   📌 **Si usas SQL Server, instala:**
   ```powershell
   Install-Package Microsoft.EntityFrameworkCore.SqlServer
   ```
5. Espera a que se complete la instalación.

---

### **2️⃣ Opción 2: Abrir la Terminal de .NET CLI en Visual Studio**
**Si prefieres usar comandos `dotnet`, usa la terminal integrada de Visual Studio.**

✅ **Pasos:**
1. **Abre Visual Studio** y carga tu proyecto.
2. **Abre la terminal** presionando:
   - **`Ctrl` + `Ñ`** (En versiones recientes de Visual Studio)
   - O ve al menú **"Ver"** → **"Terminal"**.
3. **En la terminal**, instala los paquetes necesarios con estos comandos:
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   ```
   📌 **Para SQL Server:**
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   ```
4. Espera a que se complete la instalación.

---

#### **3️⃣ Verificar que la Instalación Funcione**
**Después de instalar los paquetes, ejecuta en la terminal:**
```bash
dotnet build
```


```C#
using Microsoft.EntityFrameworkCore;

class EscuelaContext : DbContext
{
    public DbSet<Alumno> Alumnos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=escuela.db");
}
```

    - En esta línea indicamos que plantilla usaremos, en este caso una lista de clase Alumno

>```C#
>    public DbSet<Alumno> Alumnos { get; set; }
>```

    - En esta línea indicamos que usaremos SQlite

>```C#
>    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.>UseSqlite("Data Source=escuela.db");
>```

    - Aquí indicamos cómo se llamará la bbdd:

> "Data Source=escuela.db"

## 3. Creamos nuestro Program.cs

```C#
class Program
{
    static void Main(string[] args)
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
            Console.WriteLine("\nAlumnos agregados exitosamente!");

            /* SELECT */
            var alumnosList = db.Alumnos.ToList(); // Recuperamos todos los alumnos - aplicaciones PEQUEÑAS

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
            db.Alumnos.Where(alumno => alumno.Edad < 16).ExecuteDelete();

            alumnosList = db.Alumnos.ToList();

            foreach (Alumno alumno in alumnosList)
            {
                Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
            }
            ;

            /* UPDATE */
            db.Alumnos.Where(a => a.Edad > 10)
            .ExecuteUpdate(setters =>
                    setters.SetProperty(a => a.Edad, a => a.Edad + 1)
            );

        }
    }
}
```

### 1. Creamos la BBDD

    - Llamamos a using para indicar la BBDD

> using (var db = new EscuelaContext())

```C#
    /* CREATE DATA BASE */
    db.Database.EnsureCreated(); // Crear la base de datos si no existe
    Console.WriteLine("Base de datos creada exitosamente!");
```

### 2. Borramos los datos previos de la BBDD si los hubiese para no duplicarlos siempre 
```C#
    /* DELETE REGISTERS */
    db.Alumnos.ExecuteDelete();
    db.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'Alumnos';"); // ✅ Reinicia el AUTOINCREMENT
    Console.WriteLine("Base de datos limpiada exitosamente!");
```

### 3. **INSERT INTO** - Agregamos datos

    - Cuando queremos de uno en uno
```C#
    /* INSERT INTO - AGREGAR DATOS */
    db.Alumnos.Add(new Alumno { Nombre = "Juan", Edad = 10 });
    db.Alumnos.Add(new Alumno { Nombre = "Maria", Edad = 12 });
```

    - Cuando queremos varios datos a la vez
```C#
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
```

> **IMPORTANTE: ** guardar los cambios
> ```C#
>   db.SaveChanges(); // Guardamos cambios
>```
><br/>

### 4. **SELECT** - recuperación de datos

```C#
    /* SELECT */
    var alumnosList = db.Alumnos.ToList(); // Recuperamos todos los alumnos - aplicaciones PEQUEÑAS

    foreach (var alumno in alumnosList)
    {
        Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
    }
```

### **📌 Ejercicio 1: Consultar**
📌 **Objetivo:**  
**1️⃣** Mostrar solo **los nombres y las edades**.  

#### **💡 Código del Ejercicio**
```csharp
    Console.WriteLine("\nSolo nombres y edades:");
    
    // 2️⃣ Obtener solo nombres y edades
    var nombresEdades = db.Alumnos.Select(a => new { a.Nombre, a.Edad }).ToList(); // ✅ "SELECT nombre, edad FROM Alumnos"

    foreach (var alumno in nombresEdades)
    {
        Console.WriteLine($"Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
    }
```

### 5. **WHERE** - Filtrado de datos

```C#
    Console.WriteLine("\nAlumnos mayores de 18:");

    var alumnosMayores = db.Alumnos.Where(alumno => alumno.Edad >= 18).ToList();

    foreach (var alumno in alumnosMayores)
    {
        Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
    }
```

#### **📌 Ejercicio 2: Filtrar Datos con `WHERE`**
📌 **Objetivo:**  
**1️⃣** Obtener solo los alumnos con **edad mayor a 10**.  
**2️⃣** Obtener solo los alumnos en **5° grado**.  

##### **💡 Código del Ejercicio**
```csharp
    Console.WriteLine("\nAlumnos con edad mayor a 10:");

    // 1️⃣ Obtener alumnos mayores de 10 años
    var alumnosMayores = db.Alumnos.Where(a => a.Edad > 10).ToList(); // ✅ "SELECT * FROM Alumnos WHERE edad > 10"

    foreach (var alumno in alumnosMayores)
    {
        Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
    }

    Console.WriteLine("\nAlumnos en 5° grado:");

    // 2️⃣ Obtener alumnos en 5° grado
    var alumnosQuinto = db.Alumnos.Where(a => a.Grado == "5° grado").ToList(); // ✅ "SELECT * FROM Alumnos WHERE grado = '5° grado'"

    foreach (var alumno in alumnosQuinto)
    {
        Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Grado: {alumno.Grado}");
    }
```

#### **📌 Resumen**
| Concepto                      | Código en SQL                                     | Código en EF Core                                            |
| ----------------------------- | ------------------------------------------------- | ------------------------------------------------------------ |
| Obtener todos los alumnos     | `SELECT * FROM Alumnos;`                          | `db.Alumnos.ToList();`                                       |
| Obtener solo nombres y edades | `SELECT nombre, edad FROM Alumnos;`               | `db.Alumnos.Select(a => new { a.Nombre, a.Edad }).ToList();` |
| Filtrar por edad              | `SELECT * FROM Alumnos WHERE edad > 10;`          | `db.Alumnos.Where(a => a.Edad > 10).ToList();`               |
| Filtrar por grado             | `SELECT * FROM Alumnos WHERE grado = '5° grado';` | `db.Alumnos.Where(a => a.Grado == "5° grado").ToList();`     |

#### **📌 Ejercicio 3: Buscar un Alumno por Nombre (`WHERE`)**
📌 **Objetivo:**  
1️⃣ **Pedir al usuario un nombre** y buscar si existe en la base de datos.  
2️⃣ **Si existe, mostrar los datos** del alumno.  
3️⃣ **Si no existe, mostrar un mensaje indicando que no se encontró.**

##### **💡 Código del Ejercicio**
```csharp
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main()
    {
        using (var db = new EscuelaContext())
        {
            Console.Write("Ingrese el nombre del alumno a buscar: ");
            string nombreBuscado = Console.ReadLine();

            // 1️⃣ Buscar el alumno por nombre (equivalente a: SELECT * FROM Alumnos WHERE nombre = '...')
            var alumno = db.Alumnos.FirstOrDefault(a => a.Nombre == nombreBuscado);

            // 2️⃣ Mostrar el resultado
            if (alumno != null)
            {
                Console.WriteLine($"Alumno encontrado: ID={alumno.Id}, Nombre={alumno.Nombre}, Edad={alumno.Edad}");
            }
            else
            {
                Console.WriteLine("Alumno no encontrado.");
            }
        }
    }
}
```

---

#### **📌 Ejercicio 4: Contar Alumnos en Cada Grado (`GROUP BY`)**
📌 **Objetivo:**  
1️⃣ **Agrupar alumnos por grado** y contar cuántos hay en cada uno.  
2️⃣ **Mostrar el resultado en consola**.

##### **💡 Código del Ejercicio**
```csharp
    Console.WriteLine("\nNúmero de alumnos por grado:");

    // 1️⃣ Agrupar por grado y contar los alumnos
    var alumnosPorGrado = db.Alumnos
        .GroupBy(a => a.Grado)
        .Select(g => new { Grado = g.Key, Total = g.Count() })
        .ToList();

    // 2️⃣ Mostrar el resultado
    foreach (var grupo in alumnosPorGrado)
    {
        Console.WriteLine($"Grado: {grupo.Grado}, Total de alumnos: {grupo.Total}");
    }
```
📌 **Equivalente en SQL**:
```sql
SELECT grado, COUNT(*) AS total FROM Alumnos GROUP BY grado;
```

✅ **Este ejercicio refuerza el uso de `GROUP BY` en EF Core y cómo transformar los datos antes de mostrarlos.**

### 6. DELETE - Borrado de datos

```C#
    Console.WriteLine("\nBorramos a Jose");

    var alumnoJose = db.Alumnos.FirstOrDefault(alumno => alumno.Nombre == "Jose"); // Recogemos el primero que aparezca

    if (alumnoJose != null)
    {
        Console.WriteLine($"ID: {alumnoJose.Id}, Nombre: {alumnoJose.Nombre}, Edad: {alumnoJose.Edad}\n");
        // Eliminamos el dato
        db.Alumnos.Remove(alumnoJose);
        // Guardamos cambios
        db.SaveChanges();
    }

    // Comprobamos de nuevo si lo hemos eliminado
    alumnosList = db.Alumnos.ToList();

    foreach (var alumno in alumnosList)
    {
        Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
    }

    // Forma nueva
    db.Alumnos.Where(alumno => alumno.Edad < 16).ExecuteDelete();

    alumnosList = db.Alumnos.ToList();

    foreach (Alumno alumno in alumnosList)
    {
        Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
    };
```

#### **📌 Ejercicio 5: Eliminar un Alumno por Nombre (`DELETE`)**
📌 **Objetivo:**  
1️⃣ **Pedir al usuario un nombre** de alumno a eliminar.  
2️⃣ **Si el alumno existe, eliminarlo** y mostrar un mensaje de confirmación.  
3️⃣ **Si no existe, mostrar un mensaje de error.**

##### **💡 Código del Ejercicio**
```csharp
    Console.Write("Ingrese el nombre del alumno a eliminar: ");
    string nombreEliminar = Console.ReadLine();

    // 1️⃣ Buscar el alumno
    var alumno = db.Alumnos.FirstOrDefault(a => a.Nombre == nombreEliminar);

    // 2️⃣ Si existe, eliminarlo
    if (alumno != null)
    {
        db.Alumnos.Remove(alumno);
        db.SaveChanges();
        Console.WriteLine($"Alumno {nombreEliminar} eliminado correctamente.");
    }
    else
    {
        Console.WriteLine("Alumno no encontrado.");
    }
```
📌 **Equivalente en SQL**:
```sql
DELETE FROM Alumnos WHERE nombre = 'Juan';
```

#### **📌 Ejercicio 6: Eliminar Todos los Alumnos con Edad Menor a 10**
📌 **Objetivo:**  
1️⃣ **Eliminar a todos los alumnos menores de 10 años**.  
2️⃣ **Mostrar un mensaje indicando cuántos registros fueron eliminados.**

##### **💡 Código del Ejercicio**
```csharp
    // 1️⃣ Obtener alumnos menores de 10 años
    var alumnosEliminar = db.Alumnos.Where(a => a.Edad < 10).ToList();
    int totalEliminados = alumnosEliminar.Count;

    // 2️⃣ Si hay alumnos para eliminar, borrarlos
    if (totalEliminados > 0)
    {
        db.Alumnos.RemoveRange(alumnosEliminar);
        db.SaveChanges();
        Console.WriteLine($"Se eliminaron {totalEliminados} alumnos menores de 10 años.");
    }
    else
    {
        Console.WriteLine("No hay alumnos menores de 10 años para eliminar.");
    }
```
📌 **Equivalente en SQL**:
```sql
DELETE FROM Alumnos WHERE edad < 10;
```

#### **📌 Ejercicio 7: Vaciar la Tabla de Alumnos (`DELETE FROM`)**
📌 **Objetivo:**  
1️⃣ **Pedir confirmación al usuario antes de borrar todos los alumnos**.  
2️⃣ **Si confirma, eliminar todos los registros** de la tabla `Alumnos`.  
3️⃣ **Si no confirma, mostrar un mensaje y no hacer nada.**

##### **💡 Código del Ejercicio**
```csharp
    Console.Write("¿Seguro que deseas eliminar todos los alumnos? (S/N): ");
    string respuesta = Console.ReadLine();

    if (respuesta.ToUpper() == "S")
    {
        db.Alumnos.ExecuteDelete();  // ✅ Borra todos los alumnos en EF Core 7+
        Console.WriteLine("Todos los alumnos han sido eliminados.");
    }
    else
    {
        Console.WriteLine("Operación cancelada.");
    }
```
📌 **Si usas EF Core 6 o menor**, reemplaza `ExecuteDelete()` por:
```csharp
db.Alumnos.RemoveRange(db.Alumnos);
db.SaveChanges();
```
📌 **Equivalente en SQL**:
```sql
DELETE FROM Alumnos;
```

✅ **Este ejercicio es útil para aprender a interactuar con el usuario antes de eliminar datos sensibles.**

#### **📌 Resumen**
| Concepto | Código en SQL | Código en EF Core |
|----------|--------------|------------------|
| Buscar un alumno por nombre | `SELECT * FROM Alumnos WHERE nombre = 'Juan';` | `db.Alumnos.FirstOrDefault(a => a.Nombre == "Juan");` |
| Contar alumnos por grado | `SELECT grado, COUNT(*) FROM Alumnos GROUP BY grado;` | `db.Alumnos.GroupBy(a => a.Grado).Select(g => new { g.Key, Total = g.Count() }).ToList();` |
| Eliminar un alumno por nombre | `DELETE FROM Alumnos WHERE nombre = 'Carlos';` | `db.Alumnos.Remove(alumno); db.SaveChanges();` |
| Eliminar alumnos con edad menor a 10 | `DELETE FROM Alumnos WHERE edad < 10;` | `db.Alumnos.Where(a => a.Edad < 10).ExecuteDelete();` |
| Eliminar todos los registros | `DELETE FROM Alumnos;` | `db.Alumnos.ExecuteDelete();` |

### UPDATE - Actualización de datos

1️⃣ Aumentar en **1 año la edad de todos los alumnos**.  
2️⃣ Mostrar los datos actualizados en consola.  

#### **💡 Código con `ExecuteUpdate()` (Más eficiente en EF Core 7+)**
```csharp
    Console.WriteLine("Aumentando la edad de todos los alumnos...");

    // ✅ Método optimizado en EF Core 7+
    db.Alumnos.ExecuteUpdate(setters =>
        setters.SetProperty(a => a.Edad, a => a.Edad + 1));

    Console.WriteLine("Edades actualizadas. Lista de alumnos:");

    // Limpiamos caché a ver si suma bien
    db.ChangeTracker.Clear(); // ✅ Limpia la caché del DbContext. Después de ExecuteUpdate() o ExecuteDelete() → Para asegurarte de que los datos reflejan los cambios.

    // Mostrar alumnos actualizados
    var alumnos = db.Alumnos.ToList();
    foreach (var alumno in alumnos)
    {
        Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
    }
```
📌 **Equivalente en SQL**:
```sql
UPDATE Alumnos SET edad = edad + 1;
```

✅ **Usa `ExecuteUpdate()` para modificar datos sin necesidad de `SaveChanges()`.**  


#### **💡 Código con `SaveChanges()` (Método Tradicional, compatible con EF Core 6 y menor)**
```csharp
    var alumnos = db.Alumnos.ToList();

    foreach (var alumno in alumnos)
    {
        alumno.Edad += 1; // Aumentar la edad en 1
    }

    db.SaveChanges(); // ✅ Guarda los cambios en la base de datos
```
❌ **Este método es menos eficiente porque primero carga los registros en memoria.**

#### **📌 Ejercicio 1: Cambiar el Nombre de un Alumno**
📌 **Objetivo:**  
1️⃣ **Pedir al usuario** un nombre de alumno.  
2️⃣ **Si el alumno existe**, cambiar su nombre a uno nuevo.  
3️⃣ **Si no existe, mostrar un mensaje de error.**

##### **💡 Código con `FirstOrDefault()` y `SaveChanges()`**
```csharp
    Console.Write("Ingrese el nombre del alumno a modificar: ");
    string nombreAntiguo = Console.ReadLine();

    Console.Write("Ingrese el nuevo nombre: ");
    string nombreNuevo = Console.ReadLine();

    // 1️⃣ Buscar el alumno
    var alumno = db.Alumnos.FirstOrDefault(a => a.Nombre == nombreAntiguo);

    if (alumno != null)
    {
        // 2️⃣ Modificar el nombre
        alumno.Nombre = nombreNuevo;
        db.SaveChanges(); // ✅ Guarda los cambios en la base de datos
        Console.WriteLine($"Alumno actualizado: {nombreAntiguo} ahora es {nombreNuevo}.");
    }
    else
    {
        Console.WriteLine("Alumno no encontrado.");
    }
```
📌 **Equivalente en SQL**:
```sql
UPDATE Alumnos SET nombre = 'NuevoNombre' WHERE nombre = 'Juan';
```

✅ **Este método es útil cuando el usuario necesita modificar un registro específico.**

#### **📌 Comparación entre Métodos**
| Método | ¿Cómo funciona? | Rendimiento |
|--------|---------------|-------------|
| **`ExecuteUpdate()`** | Actualiza registros sin cargarlos en memoria | ✅ Más rápido |
| **`FirstOrDefault() + SaveChanges()`** | Modifica un solo registro después de cargarlo | ❌ Más lento |

✅ **Usa `ExecuteUpdate()` para actualizaciones masivas.**  
✅ **Usa `FirstOrDefault()` si necesitas actualizar solo un registro.**  


## **Introducción a la Programación Asíncrona (`async` y `await`) en C#**  
📌 **La programación asíncrona permite que una aplicación continúe funcionando mientras se ejecutan tareas que toman tiempo** (como acceder a una base de datos, leer archivos o hacer peticiones a una API).  

✅ **Ventajas de usar `async` y `await` en C#**:
- **No bloquea el hilo principal** → La aplicación sigue respondiendo mientras espera.
- **Mejor rendimiento** → Especialmente útil en operaciones de bases de datos con **Entity Framework Core**.
- **Código más limpio y legible** en comparación con usar `Thread` o `Task.Run()`.

---

### **1️⃣ ¿Cómo funciona `async` y `await` en C#?**
📌 **Reglas básicas**:
- Un **método asíncrono** debe incluir la palabra clave **`async`**.
- Dentro de un método `async`, puedes usar **`await`** para esperar la finalización de una tarea sin bloquear la ejecución.
- Los métodos asíncronos suelen devolver **`Task<T>`** en lugar de `T`.

💡 **Ejemplo básico: Simulación de una operación asíncrona**
```csharp
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {

    }
}
```
📌 **Explicación**:
- **`async Task Main()`** → Indica que el `Main()` es asíncrono.

## **2️⃣ `async` y `await` con Entity Framework Core**
📌 **Ahora aplicaremos `async` y `await` en consultas de base de datos con EF Core**.  

### **🔹 Método Sincrónico vs Asíncrono en EF Core**

#### 1. **SELECT**
📌 **Ejemplo sin `async` (bloquea el programa)**
```csharp
var alumnos = db.Alumnos.ToList();  // ❌ Bloquea el hilo principal
```

📌 **Ejemplo con `async` (no bloquea el programa)**
```csharp
var alumnos = await db.Alumnos.ToListAsync();  // ✅ No bloquea el hilo principal
```

### **3️⃣ Consultar Datos con `ToListAsync()`**
💡 **Ejemplo: Obtener todos los alumnos de forma asíncrona**
```csharp
    Console.WriteLine("Recuperando alumnos...");

    var alumnos = await db.Alumnos.ToListAsync(); // ✅ No bloquea el programa

    foreach (var alumno in alumnos)
    {
        Console.WriteLine($"ID: {alumno.Id}, Nombre: {alumno.Nombre}, Edad: {alumno.Edad}");
    }
```
📌 **Diferencia con `ToList()`**:
| Método          | Bloquea el programa? | Uso recomendado                      |
| --------------- | -------------------- | ------------------------------------ |
| `ToList()`      | ✅ Sí (bloqueante)    | Aplicaciones pequeñas                |
| `ToListAsync()` | ❌ No (asíncrono)     | Aplicaciones grandes o en producción |

---

### **4️⃣ Insertar Datos con `AddAsync()`**
📌 **Cuando agregamos datos en EF Core, `AddAsync()` evita bloqueos.**

💡 **Ejemplo: Agregar un alumno de forma asíncrona**
```csharp
    Console.WriteLine("Agregando alumno...");

    await db.Alumnos.AddAsync(new Alumno { Nombre = "Sofía", Edad = 14 });
    await db.SaveChangesAsync(); // ✅ Guarda los cambios sin bloquear

    Console.WriteLine("Alumno agregado exitosamente.");
```
📌 **Diferencia con `Add()`**:
| Método       | Bloquea el programa? | Uso recomendado       |
| ------------ | -------------------- | --------------------- |
| `Add()`      | ✅ Sí (bloqueante)    | Aplicaciones pequeñas |
| `AddAsync()` | ❌ No (asíncrono)     | Aplicaciones grandes  |

---

### **5️⃣ Actualizar Datos con `SaveChangesAsync()`**
📌 **Cuando modificamos registros, `SaveChangesAsync()` es más eficiente.**

💡 **Ejemplo: Aumentar la edad de un alumno**
```csharp
var alumno = await db.Alumnos.FirstOrDefaultAsync(a => a.Nombre == "Juan");

if (alumno != null)
{
    alumno.Edad += 1;
    await db.SaveChangesAsync(); // ✅ Guarda los cambios sin bloquear
}
```
📌 **Evita el uso de `SaveChanges()` en operaciones grandes para mejorar la respuesta de la aplicación.**

---

### **6️⃣ Eliminar Datos con `RemoveAsync()`**
📌 **Para eliminar registros en EF Core, combinamos `FirstOrDefaultAsync()` con `Remove()`.**

💡 **Ejemplo: Eliminar un alumno por nombre**
```csharp
var alumno = await db.Alumnos.FirstOrDefaultAsync(a => a.Nombre == "Carlos");

if (alumno != null)
{
    db.Alumnos.Remove(alumno);
    await db.SaveChangesAsync(); // ✅ Guarda los cambios sin bloquear
}
```
📌 **Diferencia con `SaveChanges()`**:
| Método               | Bloquea el programa? | Uso recomendado       |
| -------------------- | -------------------- | --------------------- |
| `SaveChanges()`      | ✅ Sí (bloqueante)    | Aplicaciones pequeñas |
| `SaveChangesAsync()` | ❌ No (asíncrono)     | Aplicaciones grandes  |

---

### **7️⃣ Ejercicio Completo con `async` y `await`**
📌 **Objetivo:**  
1️⃣ **Insertar un nuevo alumno de manera asíncrona.**  
2️⃣ **Actualizar la edad de un alumno.**  
3️⃣ **Eliminar a un alumno con nombre "Carlos".**  
4️⃣ **Consultar todos los alumnos de manera asíncrona.**  

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main()
    {
        using (var db = new EscuelaContext())
        {
            // 1️⃣ Insertar un nuevo alumno
            await db.Alumnos.AddAsync(new Alumno { Nombre = "Luis", Edad = 15 });
            await db.SaveChangesAsync();
            Console.WriteLine("Alumno agregado exitosamente.");

            // 2️⃣ Actualizar la edad de "Juan"
            var alumno = await db.Alumnos.FirstOrDefaultAsync(a => a.Nombre == "Juan");
            if (alumno != null)
            {
                alumno.Edad += 1;
                await db.SaveChangesAsync();
                Console.WriteLine($"Edad de {alumno.Nombre} actualizada a {alumno.Edad} años.");
            }

            // 3️⃣ Eliminar a "Carlos"
            var alumnoEliminar = await db.Alumnos.FirstOrDefaultAsync(a => a.Nombre == "Carlos");
            if (alumnoEliminar != null)
            {
                db.Alumnos.Remove(alumnoEliminar);
                await db.SaveChangesAsync();
                Console.WriteLine($"Alumno {alumnoEliminar.Nombre} eliminado.");
            }

            // 4️⃣ Mostrar alumnos actualizados
            var alumnos = await db.Alumnos.ToListAsync();
            foreach (var a in alumnos)
            {
                Console.WriteLine($"ID: {a.Id}, Nombre: {a.Nombre}, Edad: {a.Edad}");
            }
        }
    }
}
```

**📌 Resumen**<br/>
✅ **Usamos `async` y `await` para evitar que el programa se bloquee.**  
✅ **`ToListAsync()`, `AddAsync()`, `SaveChangesAsync()` mejoran el rendimiento en bases de datos grandes.**  
✅ **Las aplicaciones se vuelven más rápidas y eficientes al manejar datos asíncronamente.**  
