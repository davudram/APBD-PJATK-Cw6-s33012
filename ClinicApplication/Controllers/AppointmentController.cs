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
            return Ok(await _appointmentService.GetAppointmentList(dbo));
        }

        [HttpGet]
        [Route("{idAppointment:int}")]
        public async Task<IActionResult> GetByIdAppointment([FromRoute] int idAppointment)
        {
            return Ok(await _appointmentService.GetAppointmentDetails(idAppointment));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequestDto dto)
        {
            var newAppointment = await _appointmentService.CreateAppointment(dto);

            return Created($"/api/appointments/{newAppointment}", new {Appointment = newAppointment});
        }

        [HttpPut]
        [Route("{idAppointment:int}")]
        public async Task<IActionResult> UpdateAppointment([FromRoute] int idAppointment, [FromBody] UpdateAppointmentRequestDto dto)
        {
            return Ok(await _appointmentService.UpdateAppointment(idAppointment, dto));
        }

        [HttpDelete]
        [Route("{idAppointment:int}")]
        public async Task<IActionResult> DeleteAppointment([FromRoute] int idAppointment)
        {
            await _appointmentService.DeleteAppointment(idAppointment);

            return NoContent();
        }
    }
}
