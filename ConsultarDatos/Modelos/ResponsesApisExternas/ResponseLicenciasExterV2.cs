using ConsultarDatos.Modelos.ResponsesApisExternas.Licencias;

namespace ConsultarDatos.Modelos.ResponsesApisExternas
{
    public class ResponseLicenciasExterV2
    {
        public List<DatosLicenciaConducir> Licencias { get; set; } = new();

        public string? TipoSangre { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Celular { get; set; }
        public string? Profesion { get; set; }
        public string? DireccionUbicacion { get; set; }

        public DateTime? FechaDefuncion { get; set; }   // string del SOAP
        public string? LugarDefuncion { get; set; }
        public string? MotivoDefuncion { get; set; }

        public string? Discapacidad { get; set; }
        public DateTime? FechaMatrimonio { get; set; }
        public string? Ocupacion { get; set; }
        public string? Puntos { get; set; }
    }
}
