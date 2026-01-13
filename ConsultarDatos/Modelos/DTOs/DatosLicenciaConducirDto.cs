namespace ConsultarDatos.Modelos.DTOs
{
    public class DatosLicenciaConducirDto
    {
        // COLUMNAS PARA LICENCIA
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Celular { get; set; }
        public string? TipoLicencia { get; set; }
        public string? LicenciaFechaDesde { get; set; }
        public string? LicenciaFechaHasta { get; set; }
        public string? TipoSangre { get; set; }
        public DateTime? FechaDefuncion { get; set; }
        public string? LugarDefuncion { get; set; }
        public string? MotivoDefuncion { get; set; }
    }
}