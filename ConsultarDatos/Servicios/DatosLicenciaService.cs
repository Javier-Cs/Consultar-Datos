using ConsultarDatos.Config;
using ConsultarDatos.Modelos;
using ConsultarDatos.Modelos.ResponsesApisExternas;
using ConsultarDatos.Servicios.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ConsultarDatos.Servicios
{
    public class DatosLicenciaService : IDatosLicenciaService
    {

        private readonly HttpClient _httpClient;
        private readonly ApisConfig _urlSOAP;

        public DatosLicenciaService(HttpClient httpClient, IOptions<ApisConfig>apiOptions) {
            _httpClient = httpClient;
            _urlSOAP = apiOptions.Value;
        }


        public async Task<ResponseLicenciasExter> ConsultarDatosLicencia(string cedula)
        {
            var wrapper = new ResponseLicenciasExter();


            var licenciaRequest = new { 
                model = new { idIdentificacion = "CED", identificacion= cedula, idPersona = "" },
                code = "consultarLicencia",
                identificacion = cedula
            };


            var licenciaJson = JsonConvert.SerializeObject(licenciaRequest);
            var licenciaContent = new StringContent(licenciaJson, Encoding.UTF8, "application/json");

            try {

                var licenciaResponse = await _httpClient.PostAsync(_urlSOAP.urlApiSOAPLicencias, licenciaContent);

                if (licenciaResponse.IsSuccessStatusCode)
                {
                    var licenciaString = await licenciaResponse.Content.ReadAsStringAsync();
                    var licJson = JObject.Parse(licenciaString);
                    var retorno = licJson.SelectToken("S:Envelope.S:Body.ns0:consultarLicenciaResponse.return");

                    if (retorno?["resultado"]?["exito"]?.ToString() == "S")
                    {
                        var datosGen = retorno.SelectToken("datos.datos.datosGen");
                        var ubicacion = retorno.SelectToken("datos.datos.ubicacion");
                        var licenciasToken = retorno.SelectToken("licencias");

                        // Extracción de datos únicos para el Wrapper
                        wrapper.TipoSangre = retorno?["tipoSangre"]?.ToString() ?? retorno?.SelectToken("datos.datos.datosMAP.tipoSangre")?.ToString();
                        wrapper.Profesion = datosGen?["profesion"]?.ToString();
                        wrapper.Email = retorno?["email"]?.ToString() ?? ubicacion?["email"]?.ToString();
                        wrapper.Celular = ubicacion?["celular"]?.ToString() ?? retorno?["celular"]?.ToString();
                        wrapper.Telefono = ubicacion?["telefono"]?.ToString();
                        wrapper.DireccionUbicacion = ubicacion?["direccion"]?.ToString();
                        wrapper.LugarDefuncion = datosGen?["lugarDefuncion"]?.ToString();
                        wrapper.MotivoDefuncion = datosGen?["motivoDefuncion"]?.ToString();


                        // Lógica de Defunción SOAP
                        var fechaDefuncionSoapStr = datosGen?["fechaDefuncion"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(fechaDefuncionSoapStr) && DateTime.TryParse(fechaDefuncionSoapStr, out var fdefSoap))
                        {
                            wrapper.FechaDefuncion = fdefSoap;
                        }

                        // Normalización y Mapeo de licencias a la lista
                        JArray? listaLicencias = licenciasToken as JArray;
                        if (listaLicencias == null && licenciasToken is Object obj)
                        {
                            listaLicencias = new JArray(obj);

                        }
                        else if (listaLicencias == null)
                        {
                            listaLicencias = new JArray();
                        }

                        //var listaLicencias = (licenciasToken is JArray array) ? array :
                        //                     (licenciasToken is JObject obj) ? new JArray(obj) : new JArray();



                        if (listaLicencias.Count > 0)
                        {
                            foreach (var licenciaItem in listaLicencias)
                            {
                                // Creación de la licencia individual (Similar a tu bloque `if (listaLicencias.Count > 0)`)
                                var datosLicencia = new DatosLicenciaConducir
                                {
                                    // Datos específicos de la licencia
                                    TipoLicencia = licenciaItem?["tipo"]?.ToString(),
                                    LicenciaFechaDesde = licenciaItem?["fechaDesde"]?.ToString(),
                                    LicenciaFechaHasta = licenciaItem?["fechaHasta"]?.ToString(),

                                    // Datos únicos/consolidados (tomados del Wrapper)
                                    Email = wrapper.Email,
                                    Telefono = wrapper.Telefono,
                                    Celular = wrapper.Celular,
                                    TipoSangre = wrapper.TipoSangre,
                                    FechaDefuncion = wrapper.FechaDefuncion,
                                    LugarDefuncion = wrapper.LugarDefuncion,
                                    MotivoDefuncion = wrapper.MotivoDefuncion
                                };
                                wrapper.Licencias.Add(datosLicencia);
                            }
                        }
                        else
                        {
                            // en caso sin licencias pero con datos de contacto" (Similar a tu bloque `else`)
                            var datosContactoUnico = new DatosLicenciaConducir
                            {
                                Email = wrapper.Email,
                                Telefono = wrapper.Telefono,
                                Celular = wrapper.Celular,
                                TipoSangre = wrapper.TipoSangre,
                                FechaDefuncion = wrapper.FechaDefuncion,
                                LugarDefuncion = wrapper.LugarDefuncion,
                                MotivoDefuncion = wrapper.MotivoDefuncion
                            };

                            if (!string.IsNullOrWhiteSpace(datosContactoUnico.Email) || !string.IsNullOrWhiteSpace(datosContactoUnico.Telefono) || datosContactoUnico.FechaDefuncion.HasValue)
                            {
                                wrapper.Licencias.Add(datosContactoUnico);
                            }
                        }
                    }
                }

            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            return wrapper;
        }
    }
}
