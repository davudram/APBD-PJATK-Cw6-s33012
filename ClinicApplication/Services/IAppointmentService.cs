using ClinicApplication.DTOs;
using ClinicApplication.Models;

namespace ClinicApplication.Services
{
    public interface IAppointmentService
    {
        Task<List<AppointmentListDto>> GetAppointmentList(StatusPatientDbo dbo);
        Task<AppointmentDetailsDto> GetAppointmentDetails(int id);
        Task<CreateAppointmentRequestDto> CreateAppointment(CreateAppointmentRequestDto dto);
        Task<UpdateAppointmentRequestDto> UpdateAppointment(int id, UpdateAppointmentRequestDto dto);
        Task<bool> DeleteAppointment(int id);
    }
}
