using Infrastructure;
using Application;
using Presentation;
using WebApi.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Layers (The 'DependencyInjection.cs' files you created)
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration) 
    .AddPresentation();

// 2. Add Exception Handling
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();

// 3. Add Controllers
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Presentation.Controllers.TransactionController).Assembly);

// 4. Add API Explorer (for Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Configure Pipeline
app.UseExceptionHandler(); // This enables the Global Exception Handler

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers(); 

// Automatic migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.AppDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();