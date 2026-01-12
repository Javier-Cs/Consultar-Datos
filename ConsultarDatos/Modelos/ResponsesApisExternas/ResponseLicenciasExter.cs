namespace ConsultarDatos.Modelos.ResponsesApisExternas
{
    public class ResponseLicenciasExter
    {
        public List<DatosLicenciaConducir> Licencias { get; set; } = new List<DatosLicenciaConducir>();
        public string? TipoSangre { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Celular { get; set; }
        public string? Profesion { get; set; } // Del datosGen
        public string? DireccionUbicacion { get; set; } // De ubicacion.direccion
        public DateTime? FechaDefuncion { get; set; } // De datosGen
        public string? LugarDefuncion { get; set; } // De datosGen
        public string? MotivoDefuncion { get; set; } // De datosGen
    }
}
