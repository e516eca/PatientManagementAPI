using System.Text.Json.Serialization;

namespace PatientManagementAPI.DTOs
{
    public class HumanNameDto
    {
        [JsonPropertyName("use")]
        public string use { get; set; }

        [JsonPropertyName("family")]
        public string family { get; set; }

        [JsonPropertyName("given")]
        public List<string> given { get; set; } 
    }
}
