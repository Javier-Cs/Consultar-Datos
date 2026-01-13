namespace ConsultarDatos.Modelos.ResponsesApisExternas
{
    public class ResponseRegistroCivilExter
    {
        public string? NUI { get; set; } // NUI
        public string? Nombre { get; set; }
        public string? Sexo { get; set; } // Genero
        public string? FechaNacimiento { get; set; }
        public string? EstadoCivil { get; set; }
        public string? Conyuge { get; set; }
        public string? Nacionalidad { get; set; }
        public string? FechaCedulacion { get; set; }
        public string? Domicilio { get; set; } // Lugar Domicilio
        public string? Calle { get; set; } // Calle Domicilio
        public string? NumeroCasa { get; set; } // Numeracion Domicilio
        public string? NombreMadre { get; set; }
        public string? NombrePadre { get; set; }
        public string? LugarNacimiento { get; set; }
        public string? Instruccion { get; set; }
        public string? Profesion { get; set; }
        public string? CondicionCedulado { get; set; } // Condicion
        public string? FechaInscripcionDefuncion { get; set; } // Para consolidación
        public string? Error { get; set; } = string.Empty;
    }
}
