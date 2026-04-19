using ClinicApplication.DTOs;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ClinicApplication.Exceptions
{
    public class ExceptionMiddleware(RequestDelegate next)
    {

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                int statusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    ConflictException => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status500InternalServerError

                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var response = new ErrorResponseDto
                {
                    Message = ex.Message,
                    StatusCode = statusCode
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
