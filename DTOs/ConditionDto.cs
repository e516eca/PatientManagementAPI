namespace PatientManagementAPI.DTOs
{
    public class ConditionDto
    {
        public string Code { get; set; }          // ICD-10 or SNOMED CT code
        public string DisplayName { get; set; }   // Text description
        public DateTime? RecordedDate { get; set; }
    }
}
