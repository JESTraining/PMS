using System;
using System.Threading.Tasks;
using WebServices.Repositories.Analytics;

namespace WebServices.SharedBusiness.Analytics
{
    /// <summary>
    /// Business logic layer responsible for orchestrating analytics operations.
    /// </summary>
    public class AnalyticsProcess
    {
        private readonly AnalyticsRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticsProcess"/> class.
        /// </summary>
        /// <param name="repository">The analytics data repository.</param>
        public AnalyticsProcess(AnalyticsRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves the dashboard summary data for the current month in UTC.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the <see cref="DashboardSummaryDto"/>.</returns>
        public async Task<DashboardSummaryDto> GetDashboardDataAsync()
        {
            var today = DateTime.UtcNow;
            return await _repository.GetMonthlyDashboardSummaryAsync(today.Year, today.Month);
        }

        /// <summary>
        /// Retrieves rolling 30-day sparkline trends.
        /// </summary>
        /// <returns>A task containing the structured rolling historical timeline.</returns>
        public async Task<DashboardSparklinesDto> GetSparklinesDataAsync()
        {
            return await _repository.GetDailySparklinesAsync();
        }
    }
}