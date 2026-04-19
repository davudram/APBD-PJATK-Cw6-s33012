using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Models
{
    public class Patients
    {
        [Key]
        public int IdPatient { get; set; }
        [MaxLength(80)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(80)]
        public string LastName { get; set; } = string.Empty;
        [MaxLength(120)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
    }
}
