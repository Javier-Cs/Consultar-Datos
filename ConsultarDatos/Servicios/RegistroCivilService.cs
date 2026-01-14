using ConsultarDatos.Config;
using ConsultarDatos.Modelos;
using ConsultarDatos.Modelos.ResponsesApisExternas;
using ConsultarDatos.Servicios.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsultarDatos.Servicios
{
    public class RegistroCivilService : IRegistroCivilService
    {

        private readonly HttpClient _httpClient;
        private readonly ApisConfig _apiConfig;
        private readonly ILogger<RegistroCivilService> _logger;

        public RegistroCivilService( HttpClient httpClient, IOptions<ApisConfig> apisOptions, ILogger<RegistroCivilService> logger) {
            _apiConfig = apisOptions.Value;
            _httpClient = httpClient;
            _logger = logger;

            _httpClient.BaseAddress = _apiConfig.urlApiRestRegistriCivil;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }


        public async Task<(ResponseRegistroCivilExter? DatosPersona, string? ErrorMessage)> ObtenerInformacionDatosPersona(string Cedula, CancellationToken ct = default)
        {


            // ESTO LO PODEMOS IMPLEMENTAR EN EL CONTROLADOR

            //if (_apiConfig.urlApiRestRegistriCivil == null)
            //{
            //    return (null, "Error en la url de registro CVl");
            //}
            ////var urlRegisCivil = _apiConfig.urlApiRestRegistriCivil  + Cedula;
            //var urlRegisCivil = new Uri(_apiConfig.urlApiRestRegistriCivil,Cedula);

            try { 
                var response = await _httpClient.GetAsync(Cedula, ct);
                if (!response.IsSuccessStatusCode) {
                    return (null, $"Registro Civil respondió {(int)response.StatusCode}");
                }

                var jsonString = await response.Content.ReadAsStringAsync(ct);

                // parseo de la respuesta de la api a mi nmodelo
                //var json = JObject.Parse(jsonString); 
                //var status = (int?)json["status"];
                //var data = json["response"];

                var apiResponseBody = JsonConvert.DeserializeObject<RegistroCivilApiRespuestaBody>(jsonString);

                //bool datosInavlidos = status != 1 || data == null || data["CodigoError"]?.ToString() != "000" || data["NUI"] == null;

                if (apiResponseBody == null)
                {
                    return (null, "Respuesta vacía del Registro Civil");
                }

                bool datosInvalidos = apiResponseBody.status != 1 || 
                    apiResponseBody.response == null ||
                    !string.Equals(apiResponseBody.response.Error,"000",StringComparison.OrdinalIgnoreCase) || 
                    apiResponseBody.response.NUI == null;


                if (datosInvalidos) {

                    string mensajeError =
                        apiResponseBody.response?.Error ??
                        apiResponseBody.message ??
                        "Error desconocido del API de Registro Civil";

                    return (apiResponseBody.response, mensajeError);

                    //string? codigoError = null;
                    //if (apiResponseBody is JObject dataObject && dataObject["return"] is JObject retornos && retornos["CodigoError"] != null) { 
                    //    codigoError = retornos["CodigoError"]!.ToString();
                    //}

                    //string mensajeError = apiResponseBody.response.Error?.ToString() ??
                    //    (json["message"]?.ToString() + " (" + codigoError + " )") ??
                    //    "Error desconocido del Api RC";
                    //return (null, mensajeError);
                }

                //deserializacion al modelo
                //var rcData = apiResponseBody.ToObject<ResponseRegistroCivilExter>();
                //Cedula = rcData.NUI;
                var persona = apiResponseBody.response;
                return (persona, null);

            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error consultando Registro Civil para cédula {Cedula}", Cedula);
                return (null, $"Error al consultar datos en el Registro.");
            }
        }
    }
}
