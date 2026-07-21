using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientManagementAPI.Services;

namespace PatientManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAngular")]
    public class PatientDetailsController : ControllerBase
    {
        private readonly  IPatientDetailsService _patientDetailsService;
        private readonly IOpenAiService _openAiService;
        private readonly ILogger<PatientDetailsController> _logger;

        public PatientDetailsController(IPatientDetailsService patientDetailsService, IOpenAiService openAiService, ILogger<PatientDetailsController> logger)
        {
            _patientDetailsService = patientDetailsService;
            _openAiService = openAiService;
            _logger = logger;
        }

        //  [HttpGet("patientDetail")]
        [HttpGet]
        public async Task<IActionResult> GetPatient(string patientId)
        {
            /*
            var parameters = new Dictionary<string, string>
            {
                { "given", patientParameters.GivenName },
                { "family", patientParameters.FamilyName },
                { "birthdate", patientParameters.BirthDate }
            };
            */

          
            var patientDetails = await _patientDetailsService.GetPatientDetailsAsync(patientId);

            return Ok(patientDetails);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetPatientSummary(string patientId)
        {
            /*
            var parameters = new Dictionary<string, string>
            {
                { "given", patientParameters.GivenName },
                { "family", patientParameters.FamilyName },
                { "birthdate", patientParameters.BirthDate }
            };
            */
            var patientDetails = await _patientDetailsService.GetPatientDetailsAsync(patientId);

         
            try
            {
                _logger.LogInformation("Getting Patient Summary for patient {patientId}", patientId);
                var patientSummary = await _openAiService.GeneratePatientSummaryAsync(patientDetails);
                _logger.LogInformation("Done Getting Patient Summary for patient {patientId}", patientId);
                var responseObj = new { patientSummary = patientSummary };
                return Ok(responseObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching product {ProductId}", patientId);
                return StatusCode(500, "Internal server error");
            }

        }

    }
}
