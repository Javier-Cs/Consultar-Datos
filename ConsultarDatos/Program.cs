using ConsultarDatos.Config;
using ConsultarDatos.Servicios;
using ConsultarDatos.Servicios.Interfaces;
using ConsultarDatos.Serviciosm;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<ApisConfig>(
    builder.Configuration.GetSection("Apis"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IRegistroCivilService, RegistroCivilService>();
builder.Services.AddHttpClient<IDatosLicenciaService, DatosLicenciaService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
