using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebServices.SharedBusiness.Analytics
{
    /// <summary>
    /// Data Transfer Object representing the aggregated metrics for the dashboard summary.
    /// </summary>
    public record DashboardSummaryDto
    {
        /// <summary>
        /// Total number of appointments within the specified period.
        /// </summary>
        /// <example>150</example>
        public int TotalAppointments { get; set; }

        /// <summary>
        /// Total number of clinical encounters.
        /// </summary>
        /// <example>120</example>
        public int TotalEncounters { get; set; }

        /// <summary>
        /// Total number of newly registered patients during the specified period.
        /// </summary>
        /// <example>45</example>
        public int NewPatients { get; set; }

        /// <summary>
        /// Total earnings calculated from paid invoices.
        /// </summary>
        /// <example>25400.50</example>
        public decimal TotalEarnings { get; set; }

        /// <summary>
        /// Number of appointments successfully completed.
        /// </summary>
        /// <example>110</example>
        public int CompletedAppointments { get; set; }

        /// <summary>
        /// Number of upcoming scheduled appointments.
        /// </summary>
        /// <example>40</example>
        public int UpcomingAppointments { get; set; }

        /// <summary>
        /// Number of clinical encounters that have been fully completed.
        /// </summary>
        /// <example>115</example>
        public int CompletedEncounters { get; set; }

        /// <summary>
        /// A list of the most frequent clinical conditions registered.
        /// </summary>
        public List<ConditionChartDto> TopConditions { get; set; } = new();

        public List<MonthlyAppointmentDto> MonthlyAppointments { get; set; } = new();
        public List<CalendarDayDto> CalendarActiveDays { get; set; } = new();
    }

    public record MonthlyAppointmentDto
    {
        public string PatientName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public DateTime FullDate { get; set; } = DateTime.UtcNow.Date;
    }

    public record CalendarDayDto
    {
        public int Day { get; set; }
        public bool HasCompleted { get; set; }
        public bool HasScheduled { get; set; }
    }

    /// <summary>
    /// Data Transfer Object representing a specific clinical condition metric for charting purposes.
    /// </summary>
    public record ConditionChartDto
    {
        /// <summary>
        /// The name or clinical status of the condition.
        /// </summary>
        /// <example>Active</example>
        [Required]
        public string ConditionName { get; set; } = string.Empty;

        /// <summary>
        /// The total number of patients associated with this condition.
        /// </summary>
        /// <example>32</example>
        public int PatientCount { get; set; }
    }

    /// <summary>
    /// Data Transfer Object representing the daily time-series historical data for the dashboard sparklines.
    /// </summary>
    public record DashboardSparklinesDto
    {
        /// <summary>
        /// Sequential formatted date strings representing the last 30 days window.
        /// </summary>
        public List<string> Dates { get; set; } = new();

        /// <summary>
        /// Daily counts of appointments for the current month.
        /// </summary>
        public List<int> AppointmentsHistory { get; set; } = new();

        /// <summary>
        /// Daily counts of encounters for the current month.
        /// </summary>
        public List<int> EncountersHistory { get; set; } = new();

        /// <summary>
        /// Daily counts of new patients for the current month.
        /// </summary>
        public List<int> PatientsHistory { get; set; } = new();

        /// <summary>
        /// Daily sum of earnings for the current month.
        /// </summary>
        public List<decimal> EarningsHistory { get; set; } = new();
    }
}