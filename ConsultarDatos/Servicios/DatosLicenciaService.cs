using ConsultarDatos.Config;
using ConsultarDatos.Modelos.DTOs;
using ConsultarDatos.Modelos.ResponsesApisExternas;
using ConsultarDatos.Servicios.Interfaces;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace ConsultarDatos.Serviciosm
{
    public class DatosLicenciaService : IDatosLicenciaService
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<DatosLicenciaService> _logger;

        public DatosLicenciaService(HttpClient httpClient, IOptions<ApisConfig> apiOptions, ILogger<DatosLicenciaService> logger) {
            _httpClient = httpClient;
            _logger = logger;

            _httpClient.BaseAddress = apiOptions.Value.urlApiSOAPLicencias;
        }


        public async Task<ResponseLicenciasExter> ConsultarDatosLicencia(string cedula, CancellationToken ct = default)
        {
            var wrapper = new ResponseLicenciasExter();

            //var licenciaRequest = new { 
            //    model = new { idIdentificacion = "CED", identificacion= cedula, idPersona = "" },
            //    code = "consultarLicencia",
            //    identificacion = cedula
            //};


            //var licenciaJson = JsonConvert.SerializeObject(licenciaRequest);
            //var licenciaContent = new StringContent(licenciaJson, Encoding.UTF8, "application/json");

            try
            {
                var content = CreateResponseSOAP(cedula);

                var response = await _httpClient.PostAsync("", content, ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Licencias respondió {StatusCode} para cédula {Cedula}", response.StatusCode, cedula);
                    return wrapper;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var licJson = JObject.Parse(responseString);
                var retorno = licJson.SelectToken("S:Envelope.S:Body.ns0:consultarLicenciaResponse.return");

                if (retorno == null)
                    return wrapper;

                if (retorno?["resultado"]?["exito"]?.ToString() != "S")
                    return wrapper;

                MapearDatosGenerales(retorno, wrapper);
                MapearLicencias(retorno["licencias"], wrapper);

            }
            catch (Exception ex) {
                _logger.LogError("Error al solicitar información de la licencia para cédula {cedula}", cedula);
            }

            return wrapper;
        }

        private static StringContent CreateResponseSOAP(string cedula)
        {
            var request = new {
                model = new {
                    idIdentificacion = "CED",
                    identificacion = cedula,
                    idPersona = ""
                },
                code = "consultarLicencia",
                identificacion = cedula,
            };

            var json = JsonConvert.SerializeObject(request);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }


        private static void MapearDatosGenerales(JToken retorno, ResponseLicenciasExter wrapper){
            var datosGen = retorno.SelectToken("datos.datos.datosgen");
            var ubicacion = retorno.SelectToken("datos.datos.ubicacion");

            wrapper.TipoSangre =
                retorno?["tipoSangre"]?.ToString() ??
                retorno?.SelectToken("datos.datos.datosMAP.tipoSangre")?.ToString();

            wrapper.Profesion = 
                datosGen?["profesion"]?.ToString();
            wrapper.Email =
                retorno?["email"]?.ToString() ??
                ubicacion?["email"]?.ToString();

            wrapper.Celular =
                ubicacion?["celular"]?.ToString() ??
                retorno?["celular"]?.ToString();

            wrapper.Telefono = ubicacion?["telefono"]?.ToString();
            wrapper.DireccionUbicacion = ubicacion?["direccion"]?.ToString();
            wrapper.LugarDefuncion = datosGen?["lugarDefuncion"]?.ToString();
            wrapper.MotivoDefuncion = datosGen?["motivoDefuncion"]?.ToString();

            var fechaDefuncion = datosGen?["fechaDefuncion"]?.ToString();
            if (!string.IsNullOrWhiteSpace(fechaDefuncion) && DateTime.TryParse(fechaDefuncion, out var fechaDef))
            {
                wrapper.FechaDefuncion = fechaDef; 
            }
        }


        private static void MapearLicencias(JToken? licenciasToken, ResponseLicenciasExter wrapper) {
            JArray listaLicencias =
                licenciasToken as JArray ??
                (licenciasToken is JObject obj ? new JArray(obj) : new JArray());

            if (listaLicencias.Count == 0)
            {
                var contacto = new DatosLicenciaConducirDto {
                    Email = wrapper.Email,
                    Telefono = wrapper.Telefono,
                    Celular = wrapper.Celular,
                    TipoSangre = wrapper.TipoSangre,
                    FechaDefuncion = wrapper.FechaDefuncion,
                    LugarDefuncion = wrapper.LugarDefuncion,
                    MotivoDefuncion = wrapper.MotivoDefuncion
                };

                if (!string.IsNullOrWhiteSpace(contacto.Email) || !string.IsNullOrWhiteSpace(contacto.Telefono) ||contacto.FechaDefuncion.HasValue) {
                    wrapper.Licencias.Add(contacto);
                }

                return;
            }

            foreach (var item in listaLicencias)
            {
                var licencia = new DatosLicenciaConducirDto 
                {
                    TipoLicencia = item?["tipo"]?.ToString(),
                    LicenciaFechaDesde = item?["fechaDesde"]?.ToString(),
                    LicenciaFechaHasta = item?["fechaHasta"]?.ToString(),

                    Email = wrapper.Email,
                    Telefono = wrapper.Telefono,
                    Celular = wrapper.Celular,
                    TipoSangre = wrapper.TipoSangre,
                    FechaDefuncion = wrapper.FechaDefuncion,
                    LugarDefuncion = wrapper.LugarDefuncion,
                    MotivoDefuncion = wrapper.MotivoDefuncion
                };
                wrapper.Licencias.Add(licencia);
            }

        }

    }
}
