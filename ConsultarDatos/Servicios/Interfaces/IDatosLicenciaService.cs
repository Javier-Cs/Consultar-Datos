using ConsultarDatos.Modelos.ResponsesApisExternas;

namespace ConsultarDatos.Servicios.Interfaces
{
    public interface IDatosLicenciaService
    {
        Task<ResponseLicenciasExter> ConsultarDatosLicencia(string cedula);
    }
}
