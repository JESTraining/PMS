using Domain.Entities;
using Domain.Filters;
using Domain.SharedConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebServices.DataAccess;
using WebServices.Repositories;
using WebServices.SharedBusiness;


namespace WebServices.Controllers.Scheduling
{
    /// <summary>
    /// Represents the data required to create a new appointment from the frontend.
    /// </summary>
    public record AppointmentRequestRecord(
        DateTime StartTime,
        DateTime EndTime,
        string Reason,
        int DoctorId,
        int PatientId
    );

    /// <summary>
    /// Represents the fields that can be updated on an existing appointment.
    /// Provide StartTime+EndTime to reschedule, and/or Status to change state (e.g. "Cancelled").
    /// </summary>
    public record AppointmentUpdateRecord(
        DateTime? StartTime,
        DateTime? EndTime,
        string? Status
    );

    /// <summary>
    /// API Controller responsible for managing medical appointments and doctor schedules.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DatabaseContext _context;
        private readonly ScheduleProcess _scheduleProcess;
        private readonly AppointmentsRepository _appointmentsRepository;
        private readonly List<string> _authorizedRoles = new List<string>
        {
            UserConstants.RoleConstants.AdminRole,
            UserConstants.RoleConstants.DoctorRole,
            UserConstants.RoleConstants.RecepcionistRole
        };

        public AppointmentsController(
            IConfiguration config,
            DatabaseContext context,
            ScheduleProcess scheduleProcess,
            AppointmentsRepository appointmentsRepository)
        {
            _config = config;
            _context = context;
            _scheduleProcess = scheduleProcess;
            _appointmentsRepository = appointmentsRepository;
        }

        /// <summary>
        /// Searches for appointments based on specific filter criteria (Date, Doctor, Patient, etc.).
        /// </summary>
        /// <param name="filter">The filtering criteria.</param>
        /// <returns>A list of scheduled events matching the criteria.</returns>
        [HttpPost("search")]
        [Authorize]
        public async Task<ActionResult<List<ScheduleView>>> GetAppointments([FromBody] ScheduleFilter filter)
        {
            var validationProcess = new TokenValidationProcess(_config, _context);
            var authResult = await validationProcess.ValidateAuthorizationAsync(Request.Headers["Authorization"], _authorizedRoles);
            if (!authResult.Value.tokenIsValid)
            {
                return StatusCode(authResult.Value.errorStatus, authResult.Value.errorMessage);
            }

            var results = await _scheduleProcess.GetFilteredAppointmentsAsync(filter);
            return Ok(results);
        }

        /// <summary>
        /// Retrieves the specific details of a medical appointment.
        /// </summary>
        /// <param name="id">The unique identifier of the appointment.</param>
        [HttpGet("AppointmentDetails/{id}")]
        [Authorize]
        public async Task<ActionResult<ScheduleView>> AppointmentDetails(int id)
        {
            var authResult = await ValidateAuthorizationAsync();
            if (authResult != null) return authResult;

            var scheduleView = await _scheduleProcess.GetAppointmentDetailsAsync(id);
            return scheduleView != null ? Ok(scheduleView) : NotFound(new { message = "Appointment not found." });
        }

        /// <summary>
        /// Retrieves the specific details of a doctor's rest period.
        /// </summary>
        /// <param name="id">The unique identifier of the rest schedule.</param>
        [HttpGet("RestDetails/{id}")]
        [Authorize]
        public async Task<ActionResult<ScheduleView>> RestDetails(int id)
        {
            var authResult = await ValidateAuthorizationAsync();
            if (authResult != null) return authResult;

            var scheduleView = await _scheduleProcess.GetRestDetailsAsync(id);
            return scheduleView != null ? Ok(scheduleView) : NotFound(new { message = "Rest schedule not found." });
        }

        /// <summary>
        /// Checks whether a proposed time slot is available for a doctor (no overlap with appointments or rest periods).
        /// </summary>
        [HttpGet("availability")]
        [Authorize]
        public async Task<ActionResult> CheckAvailability([FromQuery] int doctorId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var authResult = await ValidateAuthorizationAsync();
            if (authResult != null) return authResult;

            var (isAvailable, errorMessage) = await _scheduleProcess.IsTimeSlotAvailableAsync(doctorId, start, end);
            return Ok(new { isAvailable, errorMessage });
        }

        /// <summary>
        /// Creates a new medical appointment.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UpsertRequest>> CreateAppointment([FromBody] AppointmentRequestRecord request)
        {
            var authResult = await ValidateAuthorizationAsync();
            if (authResult != null) return authResult;

            var appointment = new Appointment
            {
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Reason = request.Reason,
                Doctor = new Doctor { Id = request.DoctorId },
                Patient = new Patient { Id = request.PatientId }
            };

            var createProcessResult = await _appointmentsRepository.CreateAppointment(appointment);

            if (!createProcessResult.UpsertSuccessfull)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, createProcessResult.Message);
            }

            return Ok(createProcessResult);
        }

        /// <summary>
        /// Updates an existing appointment: reschedule (StartTime+EndTime) and/or change status (e.g. soft-cancel).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Edit(int id, [FromBody] AppointmentUpdateRecord request)
        {
            var authResult = await ValidateAuthorizationAsync();
            if (authResult != null) return authResult;

            AppointmentStatus? status = null;
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!Enum.TryParse<AppointmentStatus>(request.Status, true, out var parsedStatus))
                    return BadRequest(new { message = $"Invalid status '{request.Status}'." });
                status = parsedStatus;
            }

            var result = await _appointmentsRepository.UpdateAppointment(id, request.StartTime, request.EndTime, status);

            if (!result.UpsertSuccessfull)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
        }

        /// <summary>
        /// Deletes an existing appointment. (Pending full implementation).
        /// </summary>
        [HttpDelete("{id}")]
        //[Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Implement logic in AppointmentsRepository
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Helper method to centralize token and role validation logic.
        /// </summary>
        private async Task<ActionResult?> ValidateAuthorizationAsync()
        {
            var authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader)) return Unauthorized("Invalid authorization token.");

            var validation = new TokenRoleValidator(_context);
            var validationResult = await validation.ValidateTokenAndGetRole(authorizationHeader, _config["Jwt:Key"], _config["Jwt:Issuer"]);

            if (!validationResult.tokenIsValid)
            {
                return Unauthorized(new { Message = validationResult.message });
            }

            if (!_authorizedRoles.Contains(validationResult.role))
            {
                return StatusCode(StatusCodes.Status403Forbidden, validationResult.role + " does not have permission to execute this process.");
            }

            return null;
        }
    }
}