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

        public PatientDetailsController(IPatientDetailsService patientDetailsService, IOpenAiService openAiService)
        {
            _patientDetailsService = patientDetailsService;
            _openAiService = openAiService;
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
            

            var patientSummary = await _openAiService.GeneratePatientSummaryAsync(patientDetails);
            var responseObj = new { patientSummary = patientSummary };

            return Ok(responseObj);
        }

    }
}
