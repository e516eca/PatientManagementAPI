namespace PatientManagementAPI.DTOs
{
    public class VitalSignsDto
    {
        public string BloodPressure { get; set; } // e.g., "120/80 mmHg"
        public string HeartRate { get; set; }     // e.g., "72 bpm"
        public string Temperature { get; set; }   // e.g., "98.6 F"
        public string RespiratoryRate { get; set; } // e.g., "16 bpm"
        public string OxygenSaturation { get; set; } // e.g., "98 %"
        public string Height { get; set; }        // e.g., "175 cm"
        public string Weight { get; set; }        // e.g., "70 kg"
        public string BodyMassIndex { get; set; } // e.g., "22.9 kg/m2"
    }
}
