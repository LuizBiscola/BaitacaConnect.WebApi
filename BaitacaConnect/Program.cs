using BaitacaConnect.Data;
using BaitacaConnect.Services;
using BaitacaConnect.Services.Interfaces;
using BaitacaConnect.Repositories;
using BaitacaConnect.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework com PostgreSQL
builder.Services.AddDbContext<BaitacaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar Repositories no DI Container
builder.Services.AddScoped<IParqueRepository, ParqueRepository>();
builder.Services.AddScoped<ITrilhaRepository, TrilhaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IRelatorioVisitaRepository, RelatorioVisitaRepository>();
builder.Services.AddScoped<IFaunaFloraRepository, FaunaFloraRepository>();
builder.Services.AddScoped<IPontoInteresseRepository, PontoInteresseRepository>();

// Registrar Services no DI Container
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IParqueService, ParqueService>();
builder.Services.AddScoped<ITrilhaService, TrilhaService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IRelatorioVisitaService, RelatorioVisitaService>();
builder.Services.AddScoped<IFaunaFloraService, FaunaFloraService>();
builder.Services.AddScoped<IPontoInteresseService, PontoInteresseService>();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Baitaca Connect API",
        Version = "v1",
        Description = "API para gestão de parques naturais e reservas - Sistema Baitaca Connect"
    });

    // Agrupar endpoints por tags para melhor organização
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Baitaca Connect API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
