using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
namespace PatientManagementAPI.utilities
{
    public class FHIRUtilties () 
    {
        public static string GetPatient(string bundleJsonString)
        {

            // 1. Setup FHIR JSON options and deserialize the bundle
            JsonSerializerOptions options = new JsonSerializerOptions().ForFhir();
            Bundle bundle = JsonSerializer.Deserialize<Bundle>(bundleJsonString, options);

            // 2. Find the first entry that contains a Patient resource and cast it
            Patient patient = bundle.Entry
                .Select(entry => entry.Resource as Patient)
                .FirstOrDefault(p => p != null);

            // 3. Serialize the Patient object back into a JSON string
            string patientJson = JsonSerializer.Serialize(patient, options);
            return patientJson;
        }

        public static JsonElement? GetPatientResource(string bundleJson)
            {
            JsonElement patient = new JsonElement();


                using var doc = JsonDocument.Parse(bundleJson);

                if (!doc.RootElement.TryGetProperty("entry", out var entries))
                    return null;

                foreach (var entry in entries.EnumerateArray())
                {
                    if (entry.TryGetProperty("resource", out var resource) &&
                        resource.TryGetProperty("resourceType", out var resourceType) &&
                        resourceType.GetString() == "Patient")
                    {
                       patient = resource;
                    }
                }

                return patient;
            }

    }
}
