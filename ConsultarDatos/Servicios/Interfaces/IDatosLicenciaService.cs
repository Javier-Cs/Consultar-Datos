using ConsultarDatos.Modelos.ResponsesApisExternas;
using ConsultarDatos.Modelos.ResponsesApisExternas.Licencias;

namespace ConsultarDatos.Servicios.Interfaces
{
    public interface IDatosLicenciaService
    {
        Task<ResponseLicenciasExter> ConsultarDatosLicencia(string cedula, CancellationToken ct = default);
        Task<ResponseLicenciasExterV2> ConsultarDatosLicenciaV2(string cedula, CancellationToken ct = default);
    }
}
