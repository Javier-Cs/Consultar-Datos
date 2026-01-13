namespace ConsultarDatos.Modelos.ResponsesApisExternas
{
    public class RegistroCivilApiRespuestaBody
    {
        public int? status { get; set; } 
        public string? message { get; set; } = string.Empty;
        public ResponseRegistroCivilExter? response { get; set; }
    }
}
