using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebServices.SharedBusiness.Analytics;

namespace WebServices.Controllers.Analytics
{
    /// <summary>
    /// API Controller for retrieving system analytics and dashboard metrics.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    // [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AnalyticsProcess _analyticsProcess;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="analyticsProcess">The business process handler for analytics.</param>
        public DashboardController(AnalyticsProcess analyticsProcess)
        {
            _analyticsProcess = analyticsProcess;
        }

        /// <summary>
        /// Retrieves the monthly summary data for the main dashboard.
        /// </summary>
        /// <remarks>
        /// This endpoint calculates KPIs such as total earnings, upcoming appointments, and new patient registrations for the current UTC month.
        /// </remarks>
        /// <returns>An aggregated summary of hospital metrics.</returns>
        /// <response code="200">Returns the requested dashboard metrics.</response>
        /// <response code="500">If an internal server error occurs during metric calculation.</response>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DashboardSummaryDto>> GetMonthlySummary()
        {
            try
            {
                var summary = await _analyticsProcess.GetDashboardDataAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error trying to process analytics: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves daily historical time-series data for the dashboard sparkline charts.
        /// </summary>
        /// <remarks>
        /// Returns arrays of data representing daily activity trends for the current UTC month.
        /// </remarks>
        /// <returns>Time-series historical trends for key metrics.</returns>
        /// <response code="200">Returns the requested time-series datasets.</response>
        /// <response code="500">If an error occurs during processing.</response>
        [HttpGet("sparklines")]
        [ProducesResponseType(typeof(DashboardSparklinesDto), Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DashboardSparklinesDto>> GetMonthlySparklines()
        {
            try
            {
                var sparklines = await _analyticsProcess.GetSparklinesDataAsync();
                return Ok(sparklines);
            }
            catch (Exception ex)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError,
                    $"Error trying to process sparkline analytics: {ex.Message}");
            }
        } 
    }
}