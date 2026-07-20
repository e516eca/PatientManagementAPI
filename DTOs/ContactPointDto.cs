using System.Text.Json.Serialization;

namespace PatientManagementAPI.DTOs
{
    public class ContactPointDto
    {
        [JsonPropertyName("system")]
        public string System { get; set; } // e.g., "phone", "email"

        [JsonPropertyName("value")]
        public string Value { get; set; }  // The actual number/address

        [JsonPropertyName("use")]
        public string Use { get; set; } = "home";   // e.g., "home", "work", "mobile"
    }
}
