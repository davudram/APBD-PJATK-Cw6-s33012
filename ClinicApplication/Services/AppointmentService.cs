using ClinicApplication.DTOs;
using ClinicApplication.Exceptions;
using ClinicApplication.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ClinicApplication.Services
{
    public class AppointmentService(IConfiguration configuration) : IAppointmentService
    {
        public async Task<CreateAppointmentRequestDto> CreateAppointment(CreateAppointmentRequestDto dto)
        {
            await using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            await using (var checkCommand = new SqlCommand(
                @"SELECT COUNT(1)
                  FROM Patients
                  WHERE IdPatient = @IdPatient AND IsActive = 1", connection))
            {
                checkCommand.Parameters.AddWithValue("@IdPatient", dto.IdPatient);

                var count = (int)await checkCommand.ExecuteScalarAsync();

                if (count == 0) 
                    throw new NotFoundException($"Error! Patient with {dto.IdPatient} id not found.");
            }

            await using (var checkCommand = new SqlCommand(
                @"SELECT COUNT(1) 
                  FROM Doctors
                  WHERE IdDoctor = @IdDoctor AND IsActive = 1", connection))
            {
                checkCommand.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);

                var count = (int)await checkCommand.ExecuteScalarAsync();

                if (count == 0)
                    throw new NotFoundException($"Error! Doctor with {dto.IdDoctor} id not found!");
            }

            await using (var checkCommand = new SqlCommand(
                @"SELECT COUNT(1)
                  FROM Appointments
                  WHERE IdDoctor = @IdDoctor AND AppointmentDate = @AppointmentDate", connection))
            {
                checkCommand.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);
                checkCommand.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);

                var count = (int)await checkCommand.ExecuteScalarAsync();

                if (count > 0)
                    throw new ConflictException("Error! The doctor cannot have another appointment scheduled at exactly the same time.");
            }

            if (dto.AppointmentDate <= DateTimeOffset.UtcNow)
                throw new ConflictException("Error! The appointment date cannot be in the past.");

            if(string.IsNullOrWhiteSpace(dto.Reason))
                throw new ConflictException("Error! The reason must have data.");

            if (dto.Reason.Length > 250)
                throw new ConflictException("Error! The reason must be < 250 length.");

            await using var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText =
                @"INSERT INTO Appointments (IdPatient, IdDoctor, AppointmentDate, Status, Reason, InternalNotes, CreatedAt)
                  OUTPUT INSERTED.IdAppointment
                  VALUES (@IdPatient, @IdDoctor, @AppointmentDate, @Status, @Reason, @InternalNotes, @CreatedAt)";
            command.Parameters.AddWithValue("@IdPatient", dto.IdPatient);
            command.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);
            command.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
            command.Parameters.AddWithValue("@Status", "Scheduled");
            command.Parameters.AddWithValue("@Reason", dto.Reason);
            command.Parameters.AddWithValue("@InternalNotes", DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            var insertedId = (int)await command.ExecuteScalarAsync();

            return dto;
        }

        public async Task<UpdateAppointmentRequestDto> UpdateAppointment(int id, UpdateAppointmentRequestDto dto)
        {
            await using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            await using (var checkCommand = new SqlCommand(
                @"SELECT COUNT(1)
                  FROM Appointments
                  WHERE IdAppointment = @IdAppointment", connection))
            {
                checkCommand.Parameters.AddWithValue("@IdAppointment", id);

                var count = (int)await checkCommand.ExecuteScalarAsync();

                if (count == 0)
                    throw new NotFoundException($"Error! The appointment with {id} id not found.");
            }

            await using (var checkCommand = new SqlCommand(
                @"SELECT COUNT(1)
                  FROM Patients
                  WHERE IdPatient = @IdPatient AND IsActive = 1", connection))
            {
                checkCommand.Parameters.AddWithValue("@IdPatient", dto.IdPatient);

                var count = (int)await checkCommand.ExecuteScalarAsync();

                if (count == 0)
                    throw new NotFoundException($"Error! Patient with {dto.IdPatient} id not found.");
            }

            await using (var checkCommand = new SqlCommand(
                @"SELECT COUNT(1) 
                  FROM Doctors
                  WHERE IdDoctor = @IdDoctor AND IsActive = 1", connection))
            {
                checkCommand.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);

                var count = (int)await checkCommand.ExecuteScalarAsync();

                if (count == 0)
                    throw new NotFoundException($"Error! Doctor with {dto.IdDoctor} id not found!");
            }

            await using (var checkCommand = new SqlCommand(
               @"SELECT COUNT(1)
                  FROM Appointments
                  WHERE IdDoctor = @IdDoctor AND AppointmentDate = @AppointmentDate AND IdAppointment != @IdAppointment", connection))
            {
                checkCommand.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);
                checkCommand.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
                checkCommand.Parameters.AddWithValue("@IdAppointment", id);

                var count = (int)await checkCommand.ExecuteScalarAsync();

                if (count > 0)
                    throw new ConflictException("Error! The doctor cannot have another appointment scheduled at exactly the same time.");
            }

            if (dto.Status == "Scheduled" || dto.Status == "Completed" || dto.Status == "Cancelled")
            {
                await using var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText =
                    @"UPDATE Appointments
                  SET IdPatient = @IdPatient, IdDoctor = @IdDoctor, AppointmentDate = ISNULL(@AppointmentDate, AppointmentDate), Status = @Status, Reason = @Reason, InternalNotes = @InternalNotes
                  WHERE IdAppointment = @IdAppointment";

                if (dto.Status == "Completed")
                    command.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
                else
                    command.Parameters.AddWithValue("@AppointmentDate", DBNull.Value);

                command.Parameters.AddWithValue("@IdAppointment", id);
                command.Parameters.AddWithValue("@IdPatient", dto.IdPatient);
                command.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);
                command.Parameters.AddWithValue("@Status", dto.Status);
                command.Parameters.AddWithValue("@Reason", dto.Reason);
                command.Parameters.AddWithValue("@InternalNotes", (object?)dto.InternalNotes ?? DBNull.Value);

                var updated = await command.ExecuteNonQueryAsync();

                if (updated == 0)
                    throw new NotFoundException("Error! Not found.");
            }
            else
            {
                throw new ConflictException("Error! The status is not correct.");
            }

            return dto;
        }

        public async Task<bool> DeleteAppointment(int id)
        {
            await using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            await using (var checkCommand = new SqlCommand(
                @"SELECT Status
                  FROM Appointments
                  WHERE IdAppointment = @IdAppointment", connection))
            {
                checkCommand.Parameters.AddWithValue("@IdAppointment", id);

                var status = await checkCommand.ExecuteScalarAsync() as string;

                if(status == null)
                    throw new NotFoundException($"Error! The appointment with {id} id not found.");

                if(status == "Completed")
                    throw new ConflictException($"Error! The appointment with {id} id has status completed.");
            }

            await using var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText =
                @"DELETE FROM Appointments
                  WHERE IdAppointment = @IdAppointment";

            command.Parameters.AddWithValue("@IdAppointment", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<AppointmentDetailsDto> GetAppointmentDetails(int id)
        {
            var appointment = new AppointmentDetailsDto();

            await using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            await using (var checkCommand = new SqlCommand(
               @"SELECT COUNT(1)
                  FROM Appointments
                  WHERE IdAppointment = @IdAppointment", connection))
            {
                checkCommand.Parameters.AddWithValue("@IdAppointment", id);

                var count = (int)await checkCommand.ExecuteScalarAsync();

                if (count == 0)
                    throw new NotFoundException("Error! Not found");
            }

            await using var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = 
                @"SELECT p.Email, p.PhoneNumber, a.InternalNotes, d.LicenseNumber, a.CreatedAt
                  FROM Appointments a
                  LEFT JOIN Patients p ON a.IdPatient = p.IdPatient
                  LEFT JOIN Doctors d ON a.IdDoctor = d.IdDoctor
                  WHERE a.IdAppointment = @IdAppointment;";

            command.Parameters.AddWithValue("@IdAppointment", id);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                appointment = new AppointmentDetailsDto
                {
                    PatientEmail = reader.GetString(0),
                    PhoneNumber = reader.GetString(1),
                    InternalNotes = reader.IsDBNull(2) ? null : reader.GetString(2),
                    LicenseNumber = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4)
                };
            }

            return appointment;
        }

        public async Task<List<AppointmentListDto>> GetAppointmentList(StatusPatientDbo dbo)
        {
            var appointments = new List<AppointmentListDto>();

            await using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            await using var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = 
                @"SELECT   a.IdAppointment,    a.AppointmentDate,    a.Status,    a.Reason,    p.FirstName + N' ' + p.LastName AS PatientFullName,    p.Email AS PatientEmail
                FROM dbo.Appointments a
                LEFT JOIN dbo.Patients p ON p.IdPatient = a.IdPatient
                WHERE (@Status IS NULL OR a.Status = @Status) AND (@PatientLastName IS NULL OR p.LastName = @PatientLastName)
                ORDER BY a.AppointmentDate;";

            command.Parameters.AddWithValue("@Status", (object?)dbo.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@PatientLastName", (object?)dbo.PatientLastName ?? DBNull.Value);
                        
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                appointments.Add(new AppointmentListDto
                {
                    IdAppointment = reader.GetInt32(0),
                    AppointmentDate = reader.GetDateTime(1),
                    Status = reader.GetString(2),
                    Reason = reader.GetString(3),
                    PatientFullName = reader.GetString(4),
                    PatientEmail = reader.GetString(5)
                });
            }

            if (appointments.Count == 0)
                throw new NotFoundException("Error! Not found.");

            return appointments;
        }
    }
}