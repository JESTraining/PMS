using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebServices.DataAccess;
using static WebServices.SharedBusiness.PrescriptionProcess;

namespace WebServices.SharedBusiness
{
    public class EncounterProcess
    {
        private readonly DatabaseContext _dbContext;

        public EncounterProcess(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EncounterSummaryDto> StartEncounterAsync(int appointmentId)
        {
            var appointment = await _dbContext.DBAppointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null) throw new Exception("Appointment not found.");

            appointment.Status = AppointmentStatus.Completed;

            var encounter = new Encounter
            {
                AppointmentId = appointment.Id,
                DoctorId = appointment.Doctor.Id,
                PatientId = appointment.Patient.Id,
                Status = EncounterStatus.InProgress,
                StartTime = DateTime.UtcNow
            };

            _dbContext.Encounters.Add(encounter);
            await _dbContext.SaveChangesAsync();

            var clinicalNote = new ClinicalNote
            {
                EncounterId = encounter.Id,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.ClinicalNotes.Add(clinicalNote);
            await _dbContext.SaveChangesAsync();

            return await GetEncounterSummaryAsync(encounter.Id);
        }

        public async Task<EncounterSummaryDto> GetEncounterSummaryAsync(int encounterId)
        {
            var encounter = await _dbContext.Encounters
                .Include(e => e.Patient)
                .Include(e => e.ClinicalNote)
                .Include(e => e.Observations)
                .Include(e => e.Conditions)
                .Include(e => e.Procedures)
                .Include(e => e.Allergies)
                .Include(e => e.Prescriptions)
                .FirstOrDefaultAsync(e => e.Id == encounterId);

            if (encounter == null) throw new Exception("Encounter not found.");

            string patientName = encounter.Patient != null
                ? $"{encounter.Patient.LastName} {encounter.Patient.FirstName}"
                : string.Empty;

            // We explicitly cast to (object) so the compiler matches the right side of the ?? operator
            var observationsList = encounter.Observations?
                .Select(o => (object)new { o.Id, o.Category, o.DisplayName, o.ValueString, o.Unit })
                ?? new List<object>();

            var allergiesList = encounter.Allergies? 
                .Select(a => (object)new { a.Id, a.Substance, a.Criticality, a.Reaction })
                ?? new List<object>();

            return new EncounterSummaryDto(
                encounter.Id,
                encounter.AppointmentId,
                encounter.Status.ToString(),
                patientName,
                encounter.ClinicalNote?.Subjective,
                encounter.ClinicalNote?.Objective,
                encounter.ClinicalNote?.Assessment,
                encounter.ClinicalNote?.Plan,
                encounter.Observations?.Count ?? 0,
                encounter.Conditions?.Count ?? 0,
                encounter.Procedures?.Count ?? 0,
                encounter.Allergies?.Count ?? 0,
                encounter.Prescriptions?.Count ?? 0,
                observationsList,
                allergiesList
            );
        }

        public async Task<bool> UpdateClinicalNoteAsync(int encounterId, UpdateClinicalNoteRequest request)
        {
            var note = await _dbContext.ClinicalNotes.FirstOrDefaultAsync(n => n.EncounterId == encounterId);
            if (note == null) throw new Exception("Clinical Note not found.");

            note.Subjective = request.Subjective;
            note.Objective = request.Objective;
            note.Assessment = request.Assessment;
            note.Plan = request.Plan;
            note.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteEncounterAsync(int encounterId)
        {
            var encounter = await _dbContext.Encounters
                .Include(e => e.Appointment)
                .FirstOrDefaultAsync(e => e.Id == encounterId);

            if (encounter == null) throw new Exception("Encounter not found.");

            encounter.Status = EncounterStatus.Completed;
            encounter.EndTime = DateTime.UtcNow;

            if (encounter.Appointment != null)
            {
                encounter.Appointment.Status = AppointmentStatus.Completed; 
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddObservationAsync(int encounterId, CreateObservationDto dto)
        {
            var encounter = await _dbContext.Encounters.FindAsync(encounterId);
            if (encounter == null) return false;

            _dbContext.ClinicalObservations.Add(new ClinicalObservation
            {
                EncounterId = encounterId,
                PatientId = encounter.PatientId,
                Category = dto.Category,
                Code = "OBS",
                DisplayName = dto.DisplayName,
                ValueString = dto.ValueString,
                Unit = dto.Unit,
                EffectiveDate = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddAllergyAsync(int encounterId, CreateAllergyDto dto)
        {
            var encounter = await _dbContext.Encounters.FindAsync(encounterId);
            if (encounter == null) return false;

            var criticalityEnum = Enum.Parse<AllergyCriticality>(dto.Criticality, true);

            _dbContext.AllergyIntolerances.Add(new AllergyIntolerance
            {
                EncounterId = encounterId,
                PatientId = encounter.PatientId,
                Substance = dto.Substance,
                Criticality = criticalityEnum,
                Reaction = dto.Reaction,
                RecordedDate = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<EncounterDetailDto> GetEncounterDetailAsync(int encounterId)
        {
            var encounter = await _dbContext.Encounters
                .Include(e => e.Patient)
                .Include(e => e.Observations)
                .Include(e => e.Conditions)
                .Include(e => e.ClinicalNote)
                .Include(e => e.Appointment)
                .Include(e => e.Prescriptions)
                    .ThenInclude(p => p.Medications)
                        .ThenInclude(pm => pm.Medication)
                .FirstOrDefaultAsync(e => e.Id == encounterId);

            if (encounter == null)
                throw new Exception("Encounter not found.");

            var patientName = $"{encounter.Patient.LastName} {encounter.Patient.FirstName}";
            var appointment = encounter.Appointment;

            var observations = encounter.Observations.Select(o => new {
                o.Id,
                o.DisplayName,
                o.ValueQuantity,
                o.ValueString,
                o.Unit,
                o.EffectiveDate
            });

            var conditions = encounter.Conditions.Select(c => new {
                c.Id,
                c.Code, 
                c.DisplayName,
                c.ClinicalStatus,
                c.RecordedDate
            });

            var notes = new[] {
                new {
                    encounter.ClinicalNote?.Subjective,
                    encounter.ClinicalNote?.Objective,
                    encounter.ClinicalNote?.Assessment,
                    encounter.ClinicalNote?.Plan
                }
            };

            var prescriptions = encounter.Prescriptions.Select(p => new PrescriptionDto(
                p.Id,
                p.IssueDate,
                p.Medications.Select(m => new PrescriptionMedicationDto(
                    m.Id,
                    m.Dosage,
                    m.Refills,
                    new MedicationDto(
                        m.Medication.Id,
                        m.Medication.Name
                    )
                ))
            ));

            return new EncounterDetailDto(
                 encounter.Id,
                 patientName,

                 encounter.StartTime,
                 encounter.EndTime,
                 encounter.Status.ToString(),

                 appointment.Id,
                 appointment.Reason,
                 appointment.StartTime,
                 appointment.EndTime,

                 observations,
                 conditions,
                 notes,
                 prescriptions
             );
        }
        public async Task<bool> UpdateObservationAsync(int observationId, CreateObservationDto dto)
        {
            var obs = await _dbContext.ClinicalObservations
                .FirstOrDefaultAsync(o => o.Id == observationId);

            if (obs == null) return false;

            obs.DisplayName = dto.DisplayName;
            obs.ValueString = dto.ValueString;
            obs.Unit = dto.Unit;
            obs.Category = dto.Category;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateConditionAsync(int encounterId, int conditionId, CreateConditionDto dto)
        {
            var condition = await _dbContext.Conditions
                .FirstOrDefaultAsync(c => c.Id == conditionId && c.EncounterId == encounterId);

            if (condition == null) return false;

            condition.Code = dto.Code;
            condition.DisplayName = dto.DisplayName;
            condition.ClinicalStatus = Enum.Parse<ConditionClinicalStatus>(
                dto.ClinicalStatus,
                true
            );

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdatePrescriptionAsync(
        int encounterId,
        int prescriptionId,
        UpdatePrescriptionDto dto)
            {
                var prescription = await _dbContext.DBPrescriptions
                    .FirstOrDefaultAsync(p => p.Id == prescriptionId && p.EncounterId == encounterId);

                if (prescription == null)
                    return false;

                prescription.IssueDate = dto.IssueDate;

                await _dbContext.SaveChangesAsync();
                return true;
            }
        public async Task<bool> UpdateObservationAsync(int observationId, UpdateObservationDto dto)
        {
            var obs = await _dbContext.ClinicalObservations
                .FirstOrDefaultAsync(o => o.Id == observationId);

            if (obs == null) return false;

            obs.Category = dto.Category ?? obs.Category;
            obs.DisplayName = dto.DisplayName ?? obs.DisplayName;
            obs.ValueString = dto.ValueString ?? obs.ValueString;
            obs.Unit = dto.Unit ?? obs.Unit;

            await _dbContext.SaveChangesAsync();
            return true;
        }


    }

}