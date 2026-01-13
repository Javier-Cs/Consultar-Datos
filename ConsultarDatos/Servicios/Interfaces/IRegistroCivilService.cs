using ConsultarDatos.Modelos;
using ConsultarDatos.Modelos.ResponsesApisExternas;

namespace ConsultarDatos.Servicios.Interfaces
{
    public interface IRegistroCivilService
    {
        Task<(ResponseRegistroCivilExter? DatosPersona, string? ErrorMessage)> ObtenerInformacionDatosPersona(string Cedula);
    }
}
