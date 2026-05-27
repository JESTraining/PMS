using Domain.Entities;
using Domain.SharedConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebServices.DataAccess;
using WebServices.SharedBusiness;
using static WebServices.SharedBusiness.PrescriptionProcess;

namespace WebServices.Controllers.Encounter 
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EncounterController : ControllerBase
    {
        private readonly EncounterProcess _encounterProcess;
        private readonly IConfiguration _config;
        private readonly DatabaseContext _context;

        private readonly List<string> _authorizedRoles = new List<string>
        {
            UserConstants.RoleConstants.AdminRole,
            UserConstants.RoleConstants.DoctorRole
        };

        public EncounterController(EncounterProcess encounterProcess, IConfiguration config, DatabaseContext context)
        {
            _encounterProcess = encounterProcess;
            _config = config;
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of encounters for the authenticated doctor.
        /// </summary>
        /// <returns>A list of EncounterResponse objects.</returns>
        [HttpGet]
        public async Task<ActionResult<List<EncounterResponse>>> GetEncounters()
        {
            var validationProcess = new TokenValidationProcess(_config, _context);
            var authResult = await validationProcess.ValidateAuthorizationAsync(Request.Headers["Authorization"], _authorizedRoles);

            if (!authResult.Value.tokenIsValid)
            {
                return StatusCode(authResult.Value.errorStatus, authResult.Value.errorMessage);
            }

            var doctor = await _context.DBDoctors.FindAsync(authResult.Value.doctorId);
            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            var encounters = await _context.Encounters
                .Where(e => e.DoctorId == doctor.Id)
                .Select(e => new EncounterResponse
                {
                    EncounterId = e.Id,
                    PatientName = e.Patient.LastName + " " + e.Patient.FirstName,
                    StartTime = TimeOnly.FromDateTime(e.StartTime),
                    EndTime = e.EndTime.HasValue ? TimeOnly.FromDateTime(e.EndTime.Value) : TimeOnly.FromDateTime(e.StartTime),
                    EncounterReason = e.Appointment != null ? e.Appointment.Reason : string.Empty
                })
                .ToListAsync();

            return Ok(encounters);
        }

        /// <summary>
        /// Starts a new clinical encounter associated with an appointment.
        /// </summary>
        [HttpPost("start/{appointmentId}")]
        public async Task<IActionResult> StartEncounter(int appointmentId)
        {
            var result = await _encounterProcess.StartEncounterAsync(appointmentId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the complete summary of a specific encounter.
        /// </summary>
        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetSummary(int id)
        {
            var result = await _encounterProcess.GetEncounterSummaryAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Updates the clinical note (SOAP) for a specific encounter.
        /// </summary>
        [HttpPut("{id}/note")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateClinicalNoteRequest request)
        {
            var result = await _encounterProcess.UpdateClinicalNoteAsync(id, request);
            return Ok(result);
        }

        /// <summary>
        /// Completes the encounter and frees up the schedule.
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteEncounter(int id)
        {
            var result = await _encounterProcess.CompleteEncounterAsync(id);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _encounterProcess.GetEncounterDetailAsync(id);
            return Ok(result);
        }
        [HttpPost("{encounterId}/observations")]
        public async Task<IActionResult> AddObservation(int encounterId, CreateObservationDto dto)
        {
            var result = await _encounterProcess.AddObservationAsync(encounterId, dto);
            if (!result) return NotFound();

            return Ok();
        }
        [HttpPut("{encounterId}/observations/{observationId}")]
        public async Task<IActionResult> UpdateObservation(
        int encounterId,
        int observationId,
        [FromBody] UpdateObservationDto dto)
        {
            var result = await _encounterProcess.UpdateObservationAsync(observationId, dto);

            if (!result) return NotFound();

            return NoContent();
        }
        [HttpPut("{encounterId}/conditions/{conditionId}")]
        public async Task<IActionResult> UpdateCondition(
        int encounterId, 
        int conditionId,
        [FromBody] CreateConditionDto dto)
        {
            var result = await _encounterProcess.UpdateConditionAsync(encounterId, conditionId, dto);

            if (!result)
                return NotFound();

            return NoContent(); // 🔥 mejor práctica para PUT
        }
        [HttpPost("{encounterId}/conditions")]
        public async Task<IActionResult> AddCondition(int encounterId, [FromBody] CreateConditionDto dto)
        {
            var encounter = await _context.Encounters
                .Include(e => e.Conditions)
                .FirstOrDefaultAsync(e => e.Id == encounterId);

            if (encounter == null)
                return NotFound("Encounter not found");

            var condition = new Condition
            {
                Code = dto.Code,
                DisplayName = dto.DisplayName,
                ClinicalStatus = Enum.Parse<ConditionClinicalStatus>(
                dto.ClinicalStatus,
                true
            )
            };

            encounter.Conditions.Add(condition);

            await _context.SaveChangesAsync();

            return Ok(condition);
        }
        [HttpPost("{encounterId}/prescriptions")]
        public async Task<IActionResult> CreatePrescription(
        int encounterId,
        [FromBody] UpdatePrescriptionDto dto)
            {
                var encounter = await _context.Encounters
                    .Include(e => e.Prescriptions)
                    .FirstOrDefaultAsync(e => e.Id == encounterId);

                if (encounter == null)
                    return NotFound();

                var prescription = new Prescriptions
                {
                    IssueDate = dto.IssueDate,
                    DoctorId = encounter.DoctorId,
                    PatientId = encounter.PatientId,
                    Medications = dto.Medications.Select(m => new PrescriptionMedication
                    {
                        MedicationId = m.MedicationId,
                        Dosage = m.Dosage,
                        Refills = m.Refills
                    }).ToList()
                };

                encounter.Prescriptions.Add(prescription);

                await _context.SaveChangesAsync();

                return Ok(prescription);
            }

        [HttpPut("{encounterId}/prescriptions/{prescriptionId}")]
        public async Task<IActionResult> UpdatePrescription(
        int encounterId,
        int prescriptionId,
        [FromBody] UpdatePrescriptionDto dto)
        {
            var result = await _encounterProcess.UpdatePrescriptionAsync(
                encounterId,
                prescriptionId,
                dto
            );

            if (!result)
                return NotFound("Prescription not found for the given encounter.");

            return Ok(result);
        }
        [HttpPut("{encounterId}/appointment")]
        public async Task<IActionResult> UpdateAppointment(
        int encounterId,
        [FromBody] UpdateAppointmentDto dto)
            {
                var encounter = await _context.Encounters
                    .Include(e => e.Appointment)
                    .FirstOrDefaultAsync(e => e.Id == encounterId);

                if (encounter?.Appointment == null)
                    return NotFound();

                encounter.Appointment.Reason = dto.Reason;
                encounter.Appointment.StartTime = dto.StartTime;
                encounter.Appointment.EndTime = dto.EndTime;

                await _context.SaveChangesAsync();

                return NoContent();
        }
        [HttpDelete("{encounterId}/prescriptions/{prescriptionId}")]
        public async Task<IActionResult> DeletePrescription(int encounterId, int prescriptionId)
        {
            var prescription = await _context.DBPrescriptions
                .FirstOrDefaultAsync(p => p.Id == prescriptionId && p.EncounterId == encounterId);

            if (prescription == null)
                return NotFound();

            _context.DBPrescriptions.Remove(prescription);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{encounterId}/observations/{observationId}")]
        public async Task<IActionResult> DeleteObservation(int encounterId, int observationId)
        {
            var observation = await _context.ClinicalObservations
                .FirstOrDefaultAsync(o => o.Id == observationId && o.EncounterId == encounterId);

            if (observation == null)
                return NotFound();

            _context.ClinicalObservations.Remove(observation);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{encounterId}/conditions/{conditionId}")]
        public async Task<IActionResult> DeleteCondition(int encounterId, int conditionId)
        {
            var condition = await _context.Conditions
                .FirstOrDefaultAsync(c => c.Id == conditionId && c.EncounterId == encounterId);

            if (condition == null)
                return NotFound();

            _context.Conditions.Remove(condition);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}