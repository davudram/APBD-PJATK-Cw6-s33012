using ClinicApplication.DTOs;
using ClinicApplication.Models;
using ClinicApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments([FromQuery] StatusPatientDbo dbo)
        {
            return Ok(await _appointmentService.GetAppointmentListDto(dbo));
        }

        [HttpGet]
        [Route("{idAppointment:int}")]
        public async Task<IActionResult> GetByIdAppointment(int idAppointment)
        {
            return Ok(await _appointmentService.GetAppointmentDetailsDto(idAppointment));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequestDto dto)
        {
            var newAppointment = await _appointmentService.CreateAppointmentRequestDto(dto);

            return Created($"/api/appointments/{newAppointment}", new {Appointment = newAppointment});
        }

        [HttpPut]
        [Route("{idAppointment:int}")]
        public async Task<IActionResult> UpdateAppointment(int idAppointment, [FromBody] UpdateAppointmentRequestDto dto)
        {
            return Ok(await _appointmentService.UpdateAppointmentRequestDto(idAppointment, dto));
        }

        [HttpDelete]
        [Route("{idAppointment:int}")]
        public async Task<IActionResult> DeleteAppointment(int idAppointment)
        {
            await _appointmentService.DeleteAppointmentRequestDto(idAppointment);

            return NoContent();
        }
    }
}
