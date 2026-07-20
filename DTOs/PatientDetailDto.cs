namespace PatientManagementAPI.DTOs
{
    public class PatientDetailDto
    {
        // 1. Demographics (Top of page layout)
        public string PatientId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } // Added for contact tracking

        // 2. Vital Signs 
        public VitalSignsDto Vitals { get; set; } = new VitalSignsDto();

        // 3. Active Conditions & Diagnoses
        public List<ConditionDto> ActiveConditions { get; set; } = new List<ConditionDto>();

        // 4. Active Medications
        public List<MedicationDto> ActiveMedications { get; set; } = new List<MedicationDto>();
    }
}
