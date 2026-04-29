using Kodigo.Application.Strategies;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Kodigo.Infrastructure;        
using Kodigo.Application.Validators;
using Kodigo.Application.Interfaces;
using Kodigo.Infrastructure.Repositories;
using Kodigo.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// 1. Conexión real a PostgreSQL leyendo la variable de Docker Compose
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPromotionStrategy, DirectDiscountStrategy>();
builder.Services.AddScoped<IPromotionStrategy, PercentageStrategy>();

builder.Services.AddScoped<SeparataValidator>();
builder.Services.AddScoped<ISeparataRepository, SeparataRepository>();

builder.Services.AddScoped<PromotionService>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options => 
{
    options.WithTitle("API de Separatas - Kódigo Fuente");
});

app.UseCors("AllowReact");
app.UseAuthorization();
app.MapControllers();

app.Run();