using System.ComponentModel.DataAnnotations;

namespace PatientManagementAPI.DTOs
{
    public class UpdatePatientDto2
    {

        [Required(ErrorMessage = "Patient Id is required.")]
        public string PatientId { get; set; }

        [Required(ErrorMessage = "Given name is required.")]
        public string GivenName { get; set; }

        [Required(ErrorMessage = "Family name is required.")]
        public string FamilyName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression("^(male|female|other|unknown)$", ErrorMessage = "Gender must be 'male', 'female', 'other', or 'unknown'.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Birth date must match the format YYYY-MM-DD.")]
        public string BirthDate { get; set; }
    }
}
