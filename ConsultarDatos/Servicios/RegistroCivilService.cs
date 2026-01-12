using ConsultarDatos.Config;
using ConsultarDatos.Modelos;
using ConsultarDatos.Servicios.Interfaces;
using Microsoft.Extensions.Options;

namespace ConsultarDatos.Servicios
{
    public class RegistroCivilService : IRegistroCivilService
    {

        private readonly HttpClient _httpClient;
        private readonly ApisConfig _apiConfig;

        public RegistroCivilService( HttpClient httpClient, IOptions<ApisConfig> apisOptions) {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            _apiConfig = apisOptions.Value;
        }


        public Task<(DatosPersonaModel? DatosPersona, string ErrorMessage)> ObtenerInformacionDatosPersona(string Cedula)
        {
            return null;
        }
    }
}
