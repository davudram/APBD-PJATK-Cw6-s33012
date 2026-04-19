using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Models
{
    public class Specializations
    {
        [Key]
        public int IdSpecialization { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
