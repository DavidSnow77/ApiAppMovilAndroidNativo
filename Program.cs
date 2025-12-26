using Abarrotes.BaseDedatos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// ? CAMBIO 1: Leer DATABASE_URL de Railway o appsettings.json local
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnectionString");

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

// ? CAMBIO 2: Puerto dinámico para Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// ? CAMBIO 3: Ejecutar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AbarrotesReyesContext>();
    db.Database.Migrate(); // Esto ejecuta las migraciones al iniciar
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run(); // ? QUITA el segundo app.Run() que tenías duplicado