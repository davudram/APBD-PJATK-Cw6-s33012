using ClinicApplication.DTOs;
using ClinicApplication.Models;

namespace ClinicApplication.Services
{
    public interface IAppointmentService
    {
        Task<List<AppointmentListDto>> GetAppointmentListDto(StatusPatientDbo dbo);
        Task<AppointmentDetailsDto> GetAppointmentDetailsDto(int id);
        Task<CreateAppointmentRequestDto> CreateAppointmentRequestDto(CreateAppointmentRequestDto dto);
        Task<UpdateAppointmentRequestDto> UpdateAppointmentRequestDto(int id, UpdateAppointmentRequestDto dto);
        Task<bool> DeleteAppointmentRequestDto(int id);
    }
}
