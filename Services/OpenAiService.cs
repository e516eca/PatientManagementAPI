using Hl7.Fhir.Model;
using OpenAI;
using OpenAI.Chat;
using System.Text;
using System.Text.Json;

namespace PatientManagementAPI.Services
{
    public class OpenAiService : IOpenAiService
    {
        //   private readonly HttpClient _httpClient;
        private readonly OpenAIClient _client;
        private readonly IConfiguration _configuration;

        public OpenAiService(
            OpenAIClient client,
            IConfiguration configuration)
        {
           _client = client;
            _configuration = configuration;
        }

        public async Task<string> GeneratePatientSummaryAsync(object patient)
        {
            //  var patientDetails = await _patientDetailsService.GetPatientDetailsAsync(patientId);

            //  var apiKey = _configuration["OpenAI:ApiKey"];



            string patientJson = JsonSerializer.Serialize(
                patient,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            string prompt = $"""
You are a medical information assistant.

Create a patient-friendly summary based on the supplied JSON.

Requirements:
- Explain every vital sign.
- Explain what each measurement means.
- State whether the value appears low, normal, or high when appropriate.
- Explain the medication and condition in plain English.
- Use headings.
- Return well-formatted markdown.
- Do not diagnose.
- Include a disclaimer that this is informational only and not medical advice.

Patient Data:

{patientJson}
""";

            ChatClient chatClient = null;
            try { 
            chatClient = _client.GetChatClient("gpt-4.1");
            }
            catch(Exception ex)
            {
                return ex.StackTrace;
            }

            ChatCompletion completion =
                await chatClient.CompleteChatAsync(
                [
                    new SystemChatMessage(
                    "You are a professional medical summary assistant."),
                new UserChatMessage(prompt)
                ]);

         //   return completion.Value.Content[0].Text;
             return completion.Content[0].Text;

            /*
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    apiKey);
            */
            /*
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json =
                await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()!;
            */
        }
    }
}
