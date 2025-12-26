using Abarrotes.BaseDedatos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Construir connection string desde variables de Railway o appsettings.json
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    var host = Environment.GetEnvironmentVariable("PGHOST");
    var dbPort = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";  // ? Cambié "port" por "dbPort"
    var database = Environment.GetEnvironmentVariable("PGDATABASE");
    var user = Environment.GetEnvironmentVariable("PGUSER");
    var password = Environment.GetEnvironmentVariable("PGPASSWORD");

    if (!string.IsNullOrEmpty(host))
    {
        connectionString = $"Host={host};Port={dbPort};Database={database};Username={user};Password={password}";
    }
    else
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    }
}

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

// Puerto dinámico para Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// Ejecutar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AbarrotesReyesContext>();
    db.Database.Migrate();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();