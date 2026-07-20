using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using PatientManagementAPI.DTOs;
using PatientManagementAPI.utilities;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace PatientManagementAPI.Services
{
    public class PatientService:IPatientService
    {

        private readonly HttpClient client = new HttpClient();
        private readonly JsonValidator jsonValidator = new JsonValidator();

        string baseUrl = "https://fhir.medblocks.com/fhir/9FkG6OgxbjCaIMIKm2JHQSvGfFvvsSkz";
        string token = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJlNTE2ZWNhQGhvdG1haWwuY29tIiwidGVuYW50X2lkIjoiOUZrRzZPZ3hiakNhSU1JS20ySkhRU3ZHZkZ2dnNTa3oiLCJyb2xlIjoiVEVOQU5UX1VTRVIiLCJpYXQiOjE3ODM2MTU0ODgsImV4cCI6MTc4ODc5OTQ4OH0.qsBcjeaw-wU_N2uTG-l3K6C1pcI_U8aBHnTT8DrSfni711A1FzkrNYQwxW1cEoNg5xbVUCOuGqotrN74s2dpAw";

        public PatientService()
        {
            // Set up reuseable authentication headers
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
        }
        //  public async Task<string> CreatePatientAsync(string baseUrl, string givenName, string familyName, string gender, string birthDate)

        public async Task<string> GetPatient(string lastName)
        {
            // string patient = "";
            // Construct the full FHIR endpoint URL
            //  string requestUrl = $"{baseUrl.TrimEnd('/')}/Patient?family={patientParameters.FamilyName}&gvien={patientParameters.GivenName}";
            string requestUrl = $"{baseUrl.TrimEnd('/')}/Patient?family={lastName}";
            // JsonElement? patient = new JsonElement();
            string patient = "";

            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
            {
                // Crucial FHIR Header: Tells the server to return FHIR-compliant JSON
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));

                // Send the request
                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    // Throws an exception if the server returns an error code (4xx or 5xx)
                    response.EnsureSuccessStatusCode();

                    // Read and return the JSON payload
                    var bundel = await response.Content.ReadAsStringAsync();
                //    return bundel;
                    patient = FHIRUtilties.GetPatient(bundel);

               //    patient = FHIRUtilties.GetPatientResource(bundel);
                 //    return await response.Content.ReadAsStringAsync();                 
                }
               
            }
             return patient;

        }
        public async Task<string> UpdatePatientAsync(UpdatePatientDto dto)

        {
            // 3. Map DTO data directly to a standard FHIR R4 JSON object layout
            var fhirPatientPayload = new Dictionary<string, object>
            {
                 { "resourceType", "Patient" },
               { "id", dto.Id },
               { "gender", dto.Gender?.ToLower() }, // FHIR codes must be strictly lowercase
               { "birthDate", dto.BirthDate },     // Layout: "YYYY-MM-DD"
               { "name", dto.Name },             // Custom property names match FHIR specs directly
                 { "telecom", dto.Telecom?.Select(t => new Dictionary<string, string>
                   {
                       { "system", t.System?.ToLower() }, // Must be lowercase "phone"
                       { "value", t.Value },
                       { "use", t.Use?.ToLower() }         // e.g., "home", "mobile"
                   }).ToList()
               }

            };

            // 2. Serialize the object to JSON
            string jsonPayload = JsonSerializer.Serialize(fhirPatientPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/fhir+json");

            // 3. Construct the exact endpoint URL: [base]/Patient/[id]
            string requestUrl = $"{baseUrl.TrimEnd('/')}/Patient/{dto.Id}";

            // 4. Execute the POST request
            HttpResponseMessage response = await client.PutAsync(requestUrl, content);

            // 5. Handle the server response
            if (response.IsSuccessStatusCode)
            {
                // FHIR servers usually return the created resource or an OperationOutcome
                string responseBody = await response.Content.ReadAsStringAsync();
                //   return $"Success! Server assigned ID or Resource: {responseBody}";
                return responseBody;
            }
            else
            {
                string errorDetails = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"FHIR Server returned error ({response.StatusCode}): {errorDetails}");
            }
        }

        public async Task<string> CreatePatientAsync(PatientDto dto)

        {
            // 1. Construct the FHIR Patient resource model using anonymous types
            var fhirPatientPayload = new Dictionary<string, object>
            {
                 { "resourceType", "Patient" },              
               { "gender", dto.Gender?.ToLower() }, // FHIR codes must be strictly lowercase
               { "birthDate", dto.BirthDate },     // Layout: "YYYY-MM-DD"
               { "name", dto.Name },             // Custom property names match FHIR specs directly
                 { "telecom", dto.Telecom?.Select(t => new Dictionary<string, string>
                   {
                       { "system", t.System?.ToLower() }, // Must be lowercase "phone"
                       { "value", t.Value },
                       { "use", t.Use?.ToLower() }         // e.g., "home", "mobile"
                   }).ToList()
               }

            };

            // 2. Serialize the object to JSON
            string jsonPayload = JsonSerializer.Serialize(fhirPatientPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/fhir+json");

            // 3. Build the request URL (e.g., "https://your-fhir-server.com")
            string requestUrl = $"{baseUrl.TrimEnd('/')}/Patient";

            // 4. Execute the POST request
            HttpResponseMessage response = await client.PostAsync(requestUrl, content);

            // 5. Handle the server response
            if (response.IsSuccessStatusCode)
            {
                // FHIR servers usually return the created resource or an OperationOutcome
                string responseBody = await response.Content.ReadAsStringAsync();
             //   return $"Success! Server assigned ID or Resource: {responseBody}";
                return responseBody;
            }
            else
            {
                string errorDetails = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"FHIR Server returned error ({response.StatusCode}): {errorDetails}");
            }
        }


        public async Task<string> GetPatients()
        {
            var allPatients = new List<object>();

            // Set up reuseable authentication headers
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //   client.DefaultRequestHeaders.Accept.Clear();
            //   client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));

            //    string requestUrl = $"{baseUrl.TrimEnd('/')}/Patient?_count={pageSize}&_offset={offset}";
            string nextUrl = $"{baseUrl.TrimEnd('/')}/Patient";

            while (!string.IsNullOrEmpty(nextUrl))
            {
                HttpResponseMessage response = await client.GetAsync(nextUrl);
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(jsonString);
                JsonElement root = doc.RootElement;

                // Extract and store individual patient resources from the bundle entry array
                if (root.TryGetProperty("entry", out JsonElement entries))
                {
                    foreach (JsonElement entry in entries.EnumerateArray())
                    {
                        if (entry.TryGetProperty("resource", out JsonElement resource))
                        {
                            // Clone the element to preserve it outside the JsonDocument lifetime
                            allPatients.Add(JsonSerializer.Deserialize<object>(resource.GetRawText()));
                        }
                    }
                }

                // Extract next page URL if it exists
                nextUrl = null;
                if (root.TryGetProperty("link", out JsonElement links))
                {
                    foreach (JsonElement link in links.EnumerateArray())
                    {
                        if (link.GetProperty("relation").GetString() == "next")
                        {
                            nextUrl = link.GetProperty("url").GetString();
                            break;
                        }
                    }
                }
            }

            // Return the accumulated list as a single formatted JSON array string
            return JsonSerializer.Serialize(allPatients, new JsonSerializerOptions { WriteIndented = true });

        }



        //     public async Task<string> GetAllPatients(string baseUrl, string token)     

        /// <summary>
        /// Get all patients from FHIR server with pagination support
        /// </summary>      
        /// <param name="pageNumber">Page number (1-based indexing). Default is 1.</param>
        /// <param name="pageSize">Number of patients per page. Default is 10. Max is 100.</param>
        /// <returns>JSON string containing paginated patient data and pagination metadata</returns>
        public async Task<string> GetAllPatients(int pageNumber = 1, int pageSize = 100)
        {
            // Validate pagination parameters
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageSize < 1)
                pageSize = 10;
            if (pageSize > 100)
                pageSize = 100;

            // Calculate offset for FHIR _offset parameter (0-based)
            int offset = (pageNumber - 1) * pageSize;

            // Construct the FHIR query URL with _count and _offset parameters
            string requestUrl = $"{baseUrl.TrimEnd('/')}/Patient?_count={pageSize}&_offset={offset}";

            // Replace the anonymous object with an instance of a mutable class
            var paginatedResult = new PaginatedResult
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Patients = new List<object>(),
                TotalCount = 0,
                HasNextPage = false,
                NextUrl = null
            };

            try
            {
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(jsonString);
                JsonElement root = doc.RootElement;

                // Extract total count from the bundle
                if (root.TryGetProperty("total", out JsonElement totalElement))
                {
                    paginatedResult.TotalCount = totalElement.GetInt32();
                }

                // Extract and store individual patient resources from the bundle entry array
                if (root.TryGetProperty("entry", out JsonElement entries))
                {
                    foreach (JsonElement entry in entries.EnumerateArray())
                    {
                        if (entry.TryGetProperty("resource", out JsonElement resource))
                        {
                            // Clone the element to preserve it outside the JsonDocument lifetime
                            paginatedResult.Patients.Add(JsonSerializer.Deserialize<object>(resource.GetRawText()));
                        }
                    }
                }

                // Check if there's a next page URL
                if (root.TryGetProperty("link", out JsonElement links))
                {
                    foreach (JsonElement link in links.EnumerateArray())
                    {
                        if (link.TryGetProperty("relation", out JsonElement relationElement))
                        {
                            if (relationElement.GetString() == "next" && link.TryGetProperty("url", out JsonElement urlElement))
                            {
                                paginatedResult.NextUrl = urlElement.GetString();
                                paginatedResult.HasNextPage = true;
                                break;
                            }
                        }
                    }
                }

                return JsonSerializer.Serialize(paginatedResult, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Failed to retrieve patients from FHIR server: {ex.Message}", ex);
            }
        }

        public async Task<string> DeletePatientAsync(string patientId)
        { 
        if (string.IsNullOrWhiteSpace(patientId))
            throw new ArgumentException("Patient ID cannot be empty.", nameof(patientId));

            // Construct the FHIR resource URL (e.g., "Patient/123")
            string requestUrl = $"{baseUrl.TrimEnd('/')}/Patient/{patientId}";          

        // Send the HTTP DELETE request
        HttpResponseMessage response = await client.DeleteAsync(requestUrl);

        // Ensure Success (FHIR returns 200 OK, 204 No Content, or 404/410 if already gone)
        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"FHIR Delete failed with status {response.StatusCode}: {errorContent}");
        }

           return ("");

        }
        
      
    }
}





