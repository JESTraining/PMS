using Domain.Entities;
using WebServices.DataAccess;

namespace WebServices.Repositories
{
    public class AppointmentsRepository
    {
        private readonly DatabaseContext _dbContext;

        public AppointmentsRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UpsertRequest> CreateAppointment(Appointment appointment)
        {
            try
            {
                // Validate the appointment's time slot availability before proceeding with the creation.
                var process = new WebServices.SharedBusiness.ScheduleProcess(_dbContext);
                var availability = await process.IsTimeSlotAvailableAsync(
                    appointment.Doctor.Id,
                    appointment.StartTime,
                    appointment.EndTime);

                if (!availability.IsAvailable)
                    return new UpsertRequest { UpsertSuccessfull = false, Message = availability.ErrorMessage };

                // Search for the tracked entities in the database context
                var trackedDoctor = await _dbContext.DBDoctors.FindAsync(appointment.Doctor.Id);
                var trackedPatient = await _dbContext.DBPatients.FindAsync(appointment.Patient.Id);

                // Security check: If either the Doctor or Patient does not exist, return an error message.
                if (trackedDoctor == null)
                    return new UpsertRequest { UpsertSuccessfull = false, Message = "The specified Doctor does not exist." };
                if (trackedPatient == null)
                    return new UpsertRequest { UpsertSuccessfull = false, Message = "The specified Patient does not exist." };
                // Assign the real and tracked entities to the new appointment
                appointment.Doctor = trackedDoctor;
                appointment.Patient = trackedPatient;

                _dbContext.DBAppointments.Add(appointment);

                await _dbContext.SaveChangesAsync();

                return new UpsertRequest { UpsertSuccessfull = true, Message = "Appointment created successfully." };
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new UpsertRequest { UpsertSuccessfull = false, Message = errorMsg };
            }
        }

        /// <summary>
        /// Updates an existing appointment: reschedule (new start/end) and/or status change (e.g. soft-cancel).
        /// </summary>
        public async Task<UpsertRequest> UpdateAppointment(int id, DateTime? newStart, DateTime? newEnd, AppointmentStatus? newStatus)
        {
            try
            {
                var appointment = await _dbContext.DBAppointments.FindAsync(id);
                if (appointment == null)
                    return new UpsertRequest { UpsertSuccessfull = false, Message = "The specified Appointment does not exist." };

                // Reschedule: validate the new slot, ignoring this same appointment in the overlap check.
                if (newStart.HasValue && newEnd.HasValue)
                {
                    var process = new WebServices.SharedBusiness.ScheduleProcess(_dbContext);
                    var availability = await process.IsTimeSlotAvailableAsync(appointment.DoctorId, newStart.Value, newEnd.Value, id);

                    if (!availability.IsAvailable)
                        return new UpsertRequest { UpsertSuccessfull = false, Message = availability.ErrorMessage };

                    appointment.StartTime = newStart.Value;
                    appointment.EndTime = newEnd.Value;
                }

                if (newStatus.HasValue)
                    appointment.Status = newStatus.Value;

                await _dbContext.SaveChangesAsync();

                return new UpsertRequest { UpsertSuccessfull = true, Message = "Appointment updated successfully." };
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new UpsertRequest { UpsertSuccessfull = false, Message = errorMsg };
            }
        }
    }
}