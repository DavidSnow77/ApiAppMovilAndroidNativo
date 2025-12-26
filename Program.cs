using Abarrotes.BaseDedatos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Obtener DATABASE_URL de Railway
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Convertir formato postgresql:// a formato Npgsql
    var uri = new Uri(databaseUrl);
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]}";

    Console.WriteLine($"Connection string convertido desde DATABASE_URL");
}
else
{
    // Intentar construir desde variables individuales
    var host = Environment.GetEnvironmentVariable("PGHOST");
    var dbPort = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
    var database = Environment.GetEnvironmentVariable("PGDATABASE");
    var user = Environment.GetEnvironmentVariable("PGUSER");
    var password = Environment.GetEnvironmentVariable("PGPASSWORD");

    if (!string.IsNullOrEmpty(host))
    {
        connectionString = $"Host={host};Port={dbPort};Database={database};Username={user};Password={password}";
        Console.WriteLine("Connection string desde variables individuales");
    }
    else
    {
        // Fallback a appsettings.json
        connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
        Console.WriteLine("Usando appsettings.json");
    }
}

Console.WriteLine($"Connection string (primeros 50 chars): {connectionString?.Substring(0, Math.Min(50, connectionString?.Length ?? 0))}");

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