using ConsultarDatos.Config;
using ConsultarDatos.Modelos;
using ConsultarDatos.Modelos.ResponsesApisExternas;
using ConsultarDatos.Servicios.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

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


        public async Task<(ResponseRegistroCivilExter? DatosPersona, string? ErrorMessage)> ObtenerInformacionDatosPersona(string Cedula)
        {
            if (_apiConfig.Equals(""))
            {
                return (null, "Error en la url de registro CVl");
            }
            var urlRegisCivil = _apiConfig.urlApiRestRegistriCivil  + Cedula;

            try { 
                var response = await _httpClient.GetAsync(urlRegisCivil);
                var jsonString = await response.Content.ReadAsStringAsync();

                // parseo de la respuesta de la api a mi nmodelo
                var json = JObject.Parse(jsonString);
                var status = (int?)json["status"];
                var data = json["response"];

                bool datosInavlidos = status != 1 || data == null || data["CodigoError"]?.ToString() != "000" || data["NUI"] == null;

                if (datosInavlidos || data == null) {
                    string? codigoError = null;
                    if (data is JObject dataObject && dataObject["return"] is JObject retornos && retornos["CodigoError"] != null) { 
                        codigoError = retornos["CodigoError"]!.ToString();
                    }

                    string mensajeError = data?["Error"]?.ToString() ??
                        (json["message"]?.ToString() + " (" + codigoError + " )") ??
                        "Error desconocido del Api RC";
                    return (null, mensajeError);
                }

                //deserializacion al modelo
                var rcData = data.ToObject<ResponseRegistroCivilExter>();
                Cedula = rcData.NUI;
                return (rcData, null);

            }
            catch (Exception ex) {
                return (null, $"Error al coonsultar datos en el Registro: {ex.Message}");
            }
        }
    }
}
