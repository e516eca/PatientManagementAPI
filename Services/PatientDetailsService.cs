using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using PatientManagementAPI.DTOs;
using PatientManagementAPI.utilities;
using System.Buffers.Text;
using System.Net.Http.Headers; // add at top

namespace PatientManagementAPI.Services
{
    public class PatientDetailsService:IPatientDetailsService
    {
        private readonly FhirClient _client;
        private readonly JsonValidator jsonValidator = new JsonValidator();

        string baseUrl = "https://fhir.medblocks.com/fhir/9FkG6OgxbjCaIMIKm2JHQSvGfFvvsSkz";
        string token = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJlNTE2ZWNhQGhvdG1haWwuY29tIiwidGVuYW50X2lkIjoiOUZrRzZPZ3hiakNhSU1JS20ySkhRU3ZHZkZ2dnNTa3oiLCJyb2xlIjoiVEVOQU5UX1VTRVIiLCJpYXQiOjE3ODM2MTU0ODgsImV4cCI6MTc4ODc5OTQ4OH0.qsBcjeaw-wU_N2uTG-l3K6C1pcI_U8aBHnTT8DrSfni711A1FzkrNYQwxW1cEoNg5xbVUCOuGqotrN74s2dpAw";


        public PatientDetailsService()
        {
            var settings = new FhirClientSettings
            {
                Timeout = 30000,
                PreferredFormat = ResourceFormat.Json
            };

            _client = new FhirClient(baseUrl, settings);

            // set Authorization header (library version in your context exposes RequestHeaders)
            if (_client.RequestHeaders != null)
            {
                _client.RequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        //  private readonly HttpClient _client = new HttpClient();
     

        public async Task<PatientDetailDto> GetPatientDetailsAsync(string patientId)
        {
            var dto = new PatientDetailDto { PatientId = patientId };

            // 1. Fetch Demographics
            var patient = await _client.ReadAsync<Patient>($"Patient/{patientId}");
            if (patient != null)
            {
                dto.FullName = patient.Name.FirstOrDefault()?.ToString();
                dto.Gender = patient.Gender?.ToString();
                dto.DateOfBirth = !string.IsNullOrEmpty(patient.BirthDate)
                    ? DateTime.Parse(patient.BirthDate)
                    : null;
            }

            // 2. Fetch Vital Signs (LOINC sorting)
            var vitalsBundle = await _client.SearchAsync<Observation>(new[] {
            $"patient={patientId}",
            "category=vital-signs"
        });
            MapVitals(vitalsBundle, dto.Vitals);

            // 3. Fetch Active Conditions / Diagnoses
            var conditionBundle = await _client.SearchAsync<Condition>(new[] {
            $"patient={patientId}",
               "clinical-status=active"
        });
            foreach (var entry in conditionBundle.Entry.Select(e => e.Resource).Cast<Condition>())
            {
                dto.ActiveConditions.Add(new ConditionDto
                {
                    Code = entry.Code?.Coding?.FirstOrDefault()?.Code,
                    DisplayName = entry.Code?.Text ?? entry.Code?.Coding?.FirstOrDefault()?.Display,
                    RecordedDate = !string.IsNullOrEmpty(entry.RecordedDate) ? DateTime.Parse(entry.RecordedDate) : null
                });
            }

            // 4. Fetch Active Medications
            var medicationBundle = await _client.SearchAsync<MedicationRequest>(new[] {
            $"patient={patientId}",
            "status=active"
        });
            foreach (var entry in medicationBundle.Entry.Select(e => e.Resource).Cast<MedicationRequest>())
            {
                var medName = entry.Medication is CodeableConcept cc
                    ? (cc.Text ?? cc.Coding.FirstOrDefault()?.Display)
                    : (entry.Medication as ResourceReference)?.Display;

                dto.ActiveMedications.Add(new MedicationDto
                {
                    MedicationName = medName ?? "Unknown Medication",
                    DosageInstruction = entry.DosageInstruction.FirstOrDefault()?.Text,
                    Status = entry.Status?.ToString()
                });
            }

            return dto;
        }


        private void MapVitals(Bundle bundle, VitalSignsDto vitalsDto)
        {
            // Extract all underlying Observations inside search bundle
            var observations = bundle.Entry
                .Select(e => e.Resource)
                .Cast<Observation>()
                .OrderByDescending(o => o.Effective.ToString()) // Keep latest records
                .ToList();

            foreach (var obs in observations)
            {
                string loincCode = obs.Code?.Coding?.FirstOrDefault(c => c.System == "http://loinc.org")?.Code;
                if (string.IsNullOrEmpty(loincCode)) continue;

                string valueStr = ExtractObservationValue(obs);

                switch (loincCode)
                {
                    case "85354-9": // Blood Pressure Panel
                        if (obs.Component != null && obs.Component.Count > 0)
                        {
                            var sys = obs.Component.FirstOrDefault(c => c.Code.Coding.Any(x => x.Code == "8480-6"));
                            var dia = obs.Component.FirstOrDefault(c => c.Code.Coding.Any(x => x.Code == "8462-4"));
                            vitalsDto.BloodPressure = $"{((Quantity)sys?.Value)?.Value}/{((Quantity)dia?.Value)?.Value} mmHg";
                        }
                        break;


                    case "8867-4": if (string.IsNullOrEmpty(vitalsDto.HeartRate)) vitalsDto.HeartRate = valueStr; break;
                    case "8310-5": if (string.IsNullOrEmpty(vitalsDto.Temperature)) vitalsDto.Temperature = valueStr; break;
                    case "9279-1": if (string.IsNullOrEmpty(vitalsDto.RespiratoryRate)) vitalsDto.RespiratoryRate = valueStr; break;
                    case "59408-5": if (string.IsNullOrEmpty(vitalsDto.OxygenSaturation)) vitalsDto.OxygenSaturation = valueStr; break;
                    case "8302-2": if (string.IsNullOrEmpty(vitalsDto.Height)) vitalsDto.Height = valueStr; break;
                    case "29463-7": if (string.IsNullOrEmpty(vitalsDto.Weight)) vitalsDto.Weight = valueStr; break;
                    case "39156-5": if (string.IsNullOrEmpty(vitalsDto.BodyMassIndex)) vitalsDto.BodyMassIndex = valueStr; break;
                }
            }
        }

        private string ExtractObservationValue(Observation obs)
        {
            if (obs.Value is Quantity quantity)
                return $"{quantity.Value} {quantity.Unit}";
            return obs.Value?.ToString();
        }



    }
}
