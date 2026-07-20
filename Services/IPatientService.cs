using PatientManagementAPI.DTOs;
using System.Text.Json;

namespace PatientManagementAPI.Services
{
    public interface IPatientService
    {
        Task<string> GetPatient(string lastName);

        /// <summary>
        /// Get all patients from FHIR server with pagination support
        /// </summary>
        /// <param name="pageNumber">Page number (1-based indexing). Default is 1.</param>
        /// <param name="pageSize">Number of patients per page. Default is 10. Max is 100.</param>
        /// <returns>JSON string containing paginated patient data and pagination metadata</returns>
        Task<string> GetAllPatients(int pageNumber = 1, int pageSize = 10);
        Task<string> GetPatients();

        Task<string> CreatePatientAsync(PatientDto dto);
        Task<string> UpdatePatientAsync(UpdatePatientDto dto);
        Task<string> DeletePatientAsync(string patientId);
    }
}
