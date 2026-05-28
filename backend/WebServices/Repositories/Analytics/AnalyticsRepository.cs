using WebServices.DataAccess;
using WebServices.SharedBusiness.Analytics;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebServices.Repositories.Analytics
{
    /// <summary>
    /// Repository responsible for executing database queries related to system analytics and reporting.
    /// </summary>
    public class AnalyticsRepository
    {
        private readonly DatabaseContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticsRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for the application.</param>
        public AnalyticsRepository(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves aggregated dashboard metrics for a specific year and month.
        /// </summary>
        /// <param name="year">The target year (e.g., 2026).</param>
        /// <param name="month">The target month (1-12).</param>
        /// <returns>A <see cref="DashboardSummaryDto"/> containing the aggregated metrics for the requested period.</returns>
        public async Task<DashboardSummaryDto> GetMonthlyDashboardSummaryAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1).ToUniversalTime();
            var endDate = startDate.AddMonths(1).AddTicks(-1);

            /*var monthlyAppointments = await _context.DBAppointments
                .Where(a => a.StartTime >= startDate && a.StartTime <= endDate)
                .ToListAsync();*/
            var monthlyAppointments = await _context.DBAppointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.StartTime >= startDate && a.StartTime <= endDate)
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            var totalAppointments = monthlyAppointments.Count;
            var completedAppointments = monthlyAppointments.Count(a => a.Status == AppointmentStatus.Completed);
            var upcomingAppointments = monthlyAppointments.Count(a => a.Status == AppointmentStatus.Scheduled && a.StartTime > DateTime.UtcNow);

            var monthlyEncounters = await _context.Encounters
                .Where(e => e.StartTime >= startDate && e.StartTime <= endDate)
                .ToListAsync();

            var totalEncounters = monthlyEncounters.Count;
            var completedEncounters = monthlyEncounters.Count(e => e.Status == EncounterStatus.Completed);

            var newPatients = await _context.DBPatients
                .CountAsync(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate);

            var totalEarnings = await _context.DBInvoices
                .Where(i => i.IssuedDate >= startDate && i.IssuedDate <= endDate)
                .SumAsync(i => i.PaidAmount);

            var topConditions = await _context.Conditions
                .GroupBy(c => c.ClinicalStatus)
                .Select(g => new ConditionChartDto
                {
                    ConditionName = g.Key.ToString(),
                    PatientCount = g.Count()
                })
                .OrderByDescending(c => c.PatientCount)
                .Take(4)
                .ToListAsync();

            var todayStart = DateTime.UtcNow.Date;
            var todayEnd = todayStart.AddDays(1).AddTicks(-1);

            var todayAppointments = await _context.DBAppointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.StartTime >= todayStart && a.StartTime <= todayEnd)
                .OrderBy(a => a.StartTime)
                .Take(5) 
                .Select(a => new MonthlyAppointmentDto
                {
                    PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                    Type = a.Reason, 
                    DoctorName = "Dr. " + a.Doctor.Name,
                    Time = a.StartTime.ToString("hh:mm tt"),
                    FullDate = a.StartTime.Date
                })
                .ToListAsync();

            var calendarDays = monthlyAppointments
                .GroupBy(a => a.StartTime.Day)
                .Select(g => new CalendarDayDto
                {
                    Day = g.Key,
                    HasCompleted = g.Any(a => a.Status == AppointmentStatus.Completed),
                    HasScheduled = g.Any(a => a.Status == AppointmentStatus.Scheduled)
                })
                .ToList();

            var allAppointments = monthlyAppointments.Select(a => new MonthlyAppointmentDto
            {
                PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                Type = a.Reason,
                DoctorName = "Dr. " + a.Doctor.Name,
                Time = a.StartTime.ToString("hh:mm tt"),
                FullDate = a.StartTime.Date
            }).ToList();

            return new DashboardSummaryDto
            {
                TotalAppointments = totalAppointments,
                CompletedAppointments = completedAppointments,
                UpcomingAppointments = upcomingAppointments,
                TotalEncounters = totalEncounters,
                CompletedEncounters = completedEncounters,
                NewPatients = newPatients,
                TotalEarnings = totalEarnings,
                TopConditions = topConditions,
                MonthlyAppointments = allAppointments,
                CalendarActiveDays = calendarDays,
            };
        }

        /// <summary>
        /// Retrieves the rolling historical breakdown for key metrics over the last 30 days.
        /// </summary>
        /// <returns>A <see cref="DashboardSparklinesDto"/> with chronological datasets.</returns>
        public async Task<DashboardSparklinesDto> GetDailySparklinesAsync()
        {
            var endDate = DateTime.UtcNow.Date;
            var startDate = endDate.AddDays(-29);

            var appointments = await _context.DBAppointments
                .Where(a => a.StartTime >= startDate && a.StartTime < endDate.AddDays(1))
                .Select(a => a.StartTime)
                .ToListAsync();

            var encounters = await _context.Encounters
                .Where(e => e.StartTime >= startDate && e.StartTime < endDate.AddDays(1))
                .Select(e => e.StartTime)
                .ToListAsync();

            var patients = await _context.DBPatients
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt < endDate.AddDays(1))
                .Select(p => p.CreatedAt)
                .ToListAsync();

            var invoices = await _context.DBInvoices
                .Where(i => i.IssuedDate >= startDate && i.IssuedDate < endDate.AddDays(1))
                .Select(i => new { i.IssuedDate, i.PaidAmount })
                .ToListAsync();

            var dto = new DashboardSparklinesDto();

            for (int i = 0; i < 30; i++)
            {
                var currentDay = startDate.AddDays(i);

                dto.Dates.Add(currentDay.ToString("M/d/yyyy"));

                dto.AppointmentsHistory.Add(appointments.Count(a => a.Date == currentDay));
                dto.EncountersHistory.Add(encounters.Count(e => e.Date == currentDay));
                dto.PatientsHistory.Add(patients.Count(p => p.Date == currentDay));
                dto.EarningsHistory.Add(invoices.Where(i => i.IssuedDate.Date == currentDay).Sum(i => i.PaidAmount));
            }

            return dto;
        }
    }
}