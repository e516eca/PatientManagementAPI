namespace PatientManagementAPI.Services
{
    public interface IOpenAiService
    { 
        Task<string> GeneratePatientSummaryAsync(object patient);
    }
}
