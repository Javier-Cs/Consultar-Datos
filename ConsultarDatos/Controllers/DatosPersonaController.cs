using ConsultarDatos.Modelos;
using ConsultarDatos.Servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace ConsultarDatos.Controllers
{
    [ApiController]
    [Route("Api/v1/[controller]")]
    public class DatosPersonaController : ControllerBase
    {
        private readonly IRegistroCivilService _registroServic;

        public DatosPersonaController(IRegistroCivilService registroCivilService) {
            _registroServic = registroCivilService;
        }

        [HttpGet("{cedula}")]
        public async Task<IActionResult> GetDatosPersona(string cedula) { 
            var (rcData, rcError) = await _registroServic.ObtenerInformacionDatosPersona(cedula);
            if (rcData == null) {
                return StatusCode(503, new {message= "Fallo en la consulta de Registro Civil: " + rcError });
            }


        }
    }
}
