using PatientManagementAPI.DTOs;

namespace PatientManagementAPI.Services
{
    public interface IPatientDetailsService
    {
        Task<PatientDetailDto> GetPatientDetailsAsync(string patientId);
    }
}
