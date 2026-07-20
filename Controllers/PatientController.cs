using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientManagementAPI.DTOs;
using PatientManagementAPI.Services;

namespace PatientManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAngular")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }
       

        [HttpGet("patient")]
        public async Task<IActionResult> GetPatient(string lastName)
        {
            /*
            var parameters = new Dictionary<string, string>
            {
                { "given", patientParameters.GivenName },
                { "family", patientParameters.FamilyName },
                { "birthdate", patientParameters.BirthDate }
            };
            */
            var patient = await _patientService.GetPatient(lastName);

            return Ok(patient);
        }

        [HttpGet("all")]
        [HttpGet("~/")]
        public async Task<IActionResult> GetPatients()
        {
           //  var allPatients = await _patientService.GetAllPatients();
           var allPatients = await _patientService.GetPatients();

            return Ok(allPatients);
        }

        [HttpPost("create")]
        public async Task<IActionResult> GetPatients([FromBody] PatientDto payload)
        {
            var response = await _patientService.CreatePatientAsync(payload);

            return Ok(response);
        }


        [HttpPut("edit")]
       
        public async Task<IActionResult> EditPatient([FromBody] UpdatePatientDto payload)
        {
            var response = await _patientService.UpdatePatientAsync(payload);

            return Ok(response);
        }


        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePatients(string patientId)
        {
            var deletePatient = await _patientService.DeletePatientAsync(patientId);
            return Ok(deletePatient); 
        }


    }
}
