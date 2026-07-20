namespace PatientManagementAPI.DTOs
{
    public class UpdatePatientDto
    {
        public string ResourceType { get; set; }
        public string Id { get; set; }
        public List<HumanNameDto> Name { get; set; } // Must be a collection/list
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public List<ContactPointDto> Telecom { get; set; } // Added for phone/email

    }
}
