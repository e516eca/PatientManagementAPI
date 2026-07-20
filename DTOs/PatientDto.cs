
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace PatientManagementAPI.DTOs
{
    public class PatientDto
    {
        [JsonPropertyName("name")]
        public List<HumanNameDto> Name { get; set; } = new();

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("birthDate")]
        public string BirthDate { get; set; }

        // Adds support for the "telecom" array in your JSON
        [JsonPropertyName("telecom")]
        public List<ContactPointDto> Telecom { get; set; } = new();

    }
}
