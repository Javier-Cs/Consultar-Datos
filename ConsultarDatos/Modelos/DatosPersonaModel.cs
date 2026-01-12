namespace ConsultarDatos.Modelos
{
    public class DatosPersonaModel
    {
        public string? Cedula {  get; set; }
        public string? Nombre { get; set; }
        public DateTime? FechaNacimiento {  get; set; }
        public string? EstadoCivil { get; set; }
        public string? Conyugue { get; set; }
        public DateTime? FechaCedulacion {  get; set; }
        public string? LugarDomicilio {  get; set; }
        public string? CalleDomiciliop {  get; set; }
        public string? NumeroDomicilio {  get; set; }
        public string? NombresMadre {  get; set; }
        public string? NombresPadre {  get; set; }
        public string? LugarNacimiento {  get; set; }
        public DateTime? FechaConsulta {  get; set; }
        public DateTime? FechaExpiracion {  get; set; }
        public string? Genero {  get; set; }
        public string? Nacionalidad {  get; set; }
        public string? Instruccion {  get; set; }
        public string? ProfecionActual {  get; set; }
        public string? condicion {  get; set; }

        public List<DatosLicenciaConducir> DatosDeLicenciaDeConducir { get; set; } = new List<DatosLicenciaConducir>();

    }
}
