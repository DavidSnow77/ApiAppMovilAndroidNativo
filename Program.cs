using Abarrotes.BaseDedatos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Construir connection string con debugging
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

Console.WriteLine($"DATABASE_URL: {connectionString ?? "NULL"}");

if (string.IsNullOrEmpty(connectionString))
{
    var host = Environment.GetEnvironmentVariable("PGHOST");
    var dbPort = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
    var database = Environment.GetEnvironmentVariable("PGDATABASE");
    var user = Environment.GetEnvironmentVariable("PGUSER");
    var password = Environment.GetEnvironmentVariable("PGPASSWORD");

    Console.WriteLine($"PGHOST: {host ?? "NULL"}");
    Console.WriteLine($"PGPORT: {dbPort}");
    Console.WriteLine($"PGDATABASE: {database ?? "NULL"}");
    Console.WriteLine($"PGUSER: {user ?? "NULL"}");

    if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(database) && !string.IsNullOrEmpty(user))
    {
        connectionString = $"Host={host};Port={dbPort};Database={database};Username={user};Password={password}";
        Console.WriteLine("Connection string construido desde variables individuales");
    }
    else
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
        Console.WriteLine("Usando appsettings.json");
    }
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("No se pudo construir la connection string. Verifica las variables de entorno.");
}

Console.WriteLine($"Connection string final (ofuscada): {connectionString?.Substring(0, Math.Min(50, connectionString.Length))}...");

builder.Services.AddDbContext<AbarrotesReyesContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AbarrotesReyesContext>();
    db.Database.Migrate();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();