using Asp.Versioning;
using ConsultarDatos.Servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConsultarDatos.Controllers.ObtenerDatos.V2
{

    [ApiController]
    [ApiVersion("2.0")]
    [Route("Api/v{version:apiVersion}/ConsultarDatos")]
    public class DatosPersonaController : ControllerBase
    {
        private readonly IRegistroCivilService _registroServic;
        private readonly IDatosLicenciaService _datosLicencia;

        public DatosPersonaController(IRegistroCivilService registroCivilService, IDatosLicenciaService datosLicencia)
        {
            _registroServic = registroCivilService;
            _datosLicencia = datosLicencia;
        }




    }
}
