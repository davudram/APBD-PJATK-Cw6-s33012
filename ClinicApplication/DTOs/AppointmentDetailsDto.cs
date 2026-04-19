using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Models
{
    public class AppointmentDetailsDto
    {
        public string PatientEmail { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? InternalNotes { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
