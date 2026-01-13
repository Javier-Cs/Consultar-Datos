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
        private readonly IDatosLicenciaService _datosLicencia;

        public DatosPersonaController(IRegistroCivilService registroCivilService, IDatosLicenciaService datosLicencia)
        {
            _registroServic = registroCivilService;
            _datosLicencia = datosLicencia;
        }

        [HttpGet("{cedula}")]
        public async Task<IActionResult> GetDatosPersona(string cedula) {

            // OBTENER DATOS DE REGISTRO CIVIL (REST)
            var (rcData, rcError) = await _registroServic.ObtenerInformacionDatosPersona(cedula);
            if (rcData == null) {
                return StatusCode(503, new {message= "Fallo en la consulta de Registro Civil: " + rcError });
            }

            // OBTENER DATOS DE LICENCIAS (SOAP/Wrapper)
            var licenciaData = await _datosLicencia.ConsultarDatosLicencia(cedula);

            // --- PREPARACIÓN DE CONSOLIDACIÓN ---

            // Lógica de Defunción Consolidada (Prefiere SOAP si está disponible)
            DateTime? fechaDefuncionConsolidada = licenciaData.FechaDefuncion.HasValue
                ? licenciaData.FechaDefuncion
                : (DateTime.TryParse(rcData.FechaInscripcionDefuncion, out var fdef) ? fdef : (DateTime?)null);

            // Lógica de Calle Domicilio Consolidada (Mueve tu lógica de HashSet)
            string calleDomicilioConsolidada = ConsolidarDirecciones(rcData.Calle, licenciaData.DireccionUbicacion);

            // Lógica de Profesión Consolidada (Mueve tu lógica de concatenación)
            string profesionConsolidada = ConsolidarProfesiones(rcData.Profesion, licenciaData.Profesion);

            // MAPEANDO AL MODELO FINAL (ConsultaPersona) ---

            var persona = new DatosPersonaModel
            {
                // Mapeo directo de Registro Civil
                Cedula = rcData.NUI,
                Nombre = rcData.Nombre,
                Genero = rcData.Sexo,
                FechaNacimiento = DateTime.TryParse(rcData.FechaNacimiento, out var fnac) ? fnac : null,
                EstadoCivil = rcData.EstadoCivil,
                Conyuge = rcData.Conyuge,
                Nacionalidad = rcData.Nacionalidad,
                FechaCedulacion = DateTime.TryParse(rcData.FechaCedulacion, out var fced) ? fced : null,
                LugarDomicilio = rcData.Domicilio,
                NumeracionDomicilio = rcData.NumeroCasa,
                NombreMadre = rcData.NombreMadre,
                NombrePadre = rcData.NombrePadre,
                LugarNacimiento = rcData.LugarNacimiento,
                Instruccion = rcData.Instruccion,
                Condicion = rcData.CondicionCedulado,

                // Consolidación de campos
                CalleDomicilio = calleDomicilioConsolidada,
                Profesion = profesionConsolidada,

                // Asignación de la lista completa de licencias (del LicenciaWrapper)
                licenciasDeConducir = licenciaData.Licencias,

                // Datos de auditoría
                FechaConsulta = DateTime.UtcNow,
                FechaExpira = DateTime.Today // Manteniendo tu lógica original
            };

            return Ok(persona);
        }


        private string ConsolidarDirecciones(string? rcCalle, string? licDir)
        {
            var direcciones = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // rcCalle viene de data["Calle"] y licDir viene de ubicacion?["direccion"]
            if (!string.IsNullOrWhiteSpace(rcCalle)) direcciones.Add(rcCalle.Trim());
            if (!string.IsNullOrWhiteSpace(licDir)) direcciones.Add(licDir.Trim());

            // Tu lógica original también usaba "data["Domicilio"]" (LugarDomicilio), "retorno?.SelectToken("datos.datos.datosMAP.direccionDomicilio")" y "retorno?["direccion"]"
            // Si necesitas esos, deben ser pasados como argumentos o extraídos en el servicio.

            // Si quieres recrear EXACTAMENTE tu lógica original con las 4 fuentes, necesitas ajustar el LicenciasService para que el wrapper las contenga todas.
            // Por simplicidad, aquí solo consolidamos las que vienen directamente de RC y del wrapper de Licencias.

            return string.Join("; ", direcciones.Where(d => !string.IsNullOrWhiteSpace(d)));
        }

        private string? ConsolidarProfesiones(string? rcProf, string? licProf)
        {
            var profesiones = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // rcProf viene de data["Profesion"] y licProf viene de datosGen?["profesion"]
            if (!string.IsNullOrWhiteSpace(rcProf)) profesiones.Add(rcProf.Trim());
            if (!string.IsNullOrWhiteSpace(licProf)) profesiones.Add(licProf.Trim());

            return string.Join("; ", profesiones.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }
}
