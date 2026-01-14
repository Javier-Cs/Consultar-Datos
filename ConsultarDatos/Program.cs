using Asp.Versioning;
using ConsultarDatos.Config;
using ConsultarDatos.Servicios;
using ConsultarDatos.Servicios.Interfaces;
using ConsultarDatos.Serviciosm;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;




var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.Configure<ApisConfig>(builder.Configuration.GetSection("Apis"));



builder.Services.AddApiVersioning( options => 
{
    options.DefaultApiVersion = new ApiVersion(1,0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();

}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
}); 

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
