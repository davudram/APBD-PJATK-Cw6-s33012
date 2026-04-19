using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Models
{
    public class Doctors
    {
        [Key]
        public int IdDoctor { get; set; }
        public int IdSpecialization { get; set; }
        [MaxLength(80)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(80)]
        public string LastName { get; set; } = string.Empty;
        [MaxLength(40)]
        public string LicenseNumber {  get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
