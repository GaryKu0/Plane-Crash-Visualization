using Microsoft.EntityFrameworkCore;
using Plane_Crash_Visualization.Data;
using Plane_Crash_Visualization.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddDbContext<Plane_Crash_Visualization.Data.CrashDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HTTP client and geocoding service
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>();
builder.Services.AddScoped<IGeocodingService, GeocodingService>();

// Add aircraft analysis services
builder.Services.AddScoped<AircraftTypeService>();
builder.Services.AddScoped<DatabaseUpdateService>();
builder.Services.AddScoped<IContinentService, ContinentService>();


var app = builder.Build();

// Automatischer SQL-Skript-Import für (localdb)\mssqllocaldb, falls DB nicht existiert
try
{
    var masterConnStr = "Server=(localdb)\\mssqllocaldb;Integrated Security=true;Database=master";
    using (var connection = new Microsoft.Data.SqlClient.SqlConnection(masterConnStr))
    {
        connection.Open();
        // Prüfen, ob PlaneCrashDB existiert
        var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = 'PlaneCrashDB'";
        int exists = (int)checkCmd.ExecuteScalar();
        if (exists == 0)
        {
            // Skript einlesen (ohne FILENAME=... Zeilen!)
            var script = System.IO.File.ReadAllText("Data/PlaneCrashDB.sql");
            // Optional: Zeilen mit FILENAME entfernen
            script = System.Text.RegularExpressions.Regex.Replace(script, @"FILENAME\s*=\s*N'[^']*'", "");
            // Skript in einzelne Befehle splitten
            var batches = script.Split(new[] {"GO\r\n", "GO\n", "GO "}, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var batch in batches)
            {
                if (!string.IsNullOrWhiteSpace(batch))
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = batch;
                        try { cmd.ExecuteNonQuery(); } catch { /* Fehler ignorieren, falls z.B. DB schon existiert */ }
                    }
                }
            }
            Console.WriteLine("[INFO] Die Datenbank wurde per Skript neu angelegt.");
        }
        else
        {
            Console.WriteLine("[INFO] Die Datenbank PlaneCrashDB ist bereits vorhanden.");
        }

        // Nach dem Import: Anzahl der Einträge in dbo.Crashes ausgeben
        try
        {
            using (var checkConn = new Microsoft.Data.SqlClient.SqlConnection("Server=(localdb)\\mssqllocaldb;Integrated Security=true;Database=PlaneCrashDB"))
            {
                checkConn.Open();
                var cmd = checkConn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM dbo.Crashes";
                int count = (int)cmd.ExecuteScalar();
                Console.WriteLine($"[INFO] Es sind {count} Einträge in dbo.Crashes vorhanden.");
            }
        }
        catch (Exception ex2)
        {
            Console.WriteLine($"[WARN] Konnte die Tabelle nicht prüfen: {ex2.Message}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[WARN] Konnte das SQL-Skript nicht automatisch ausführen: {ex.Message}");
}

// Automatically import CSV data on startup
/*using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CrashDbContext>();
    dbContext.Database.Migrate(); // Ensure DB is created and up to date
    var importer = new CsvImporter(dbContext);
    importer.ImportCrashesFromCsv("Data/plane_crash_data.csv");
}*/

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
