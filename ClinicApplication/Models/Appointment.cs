using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Models
{
    public class Appointment
    {
        [Key]
        public int IdAppointment { get; set; }
        public int IdPatient { get; set; }
        public int IdDoctor { get; set; }
        public DateTime AppointmentDate { get; set; }
        [MaxLength(30)]
        public string Status { get; set; } = string.Empty;
        [MaxLength(250)]
        public string Reason { get; set; } = string.Empty;
        [MaxLength(500)]
        public string InternalNotes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
