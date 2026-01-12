using ConsultarDatos.Modelos;

namespace ConsultarDatos.Servicios.Interfaces
{
    public interface IRegistroCivilService
    {
        Task<(DatosPersonaModel? DatosPersona, string ErrorMessage)> ObtenerInformacionDatosPersona(string Cedula);
    }
}
