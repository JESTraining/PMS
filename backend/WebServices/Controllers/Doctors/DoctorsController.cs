using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using WebServices.DataAccess;
using WebServices.SharedBusiness;

namespace WebServices.Controllers.Doctors
{
    /// <summary>
    /// API Controller responsible for managing doctors' information.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly DoctorProcess _doctorProcess;

        public DoctorsController(DatabaseContext dbContext)
        {
            _doctorProcess = new DoctorProcess(dbContext);
        }

        /// <summary>
        /// Gets a list of all doctors. 
        /// This endpoint can be used to retrieve the complete list of doctors without any search criteria.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            var doctors = await _doctorProcess.SearchDoctorsAsync("");
            return Ok(doctors);
        }

        /// <summary>
        /// Searches for doctors by matching text against their name or exact ID.
        /// </summary>
        /// <param name="searchTerm">The ID or Name to search for.</param>
        /// <returns>A list of doctors matching the search criteria.</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Doctor>>> SearchDoctors([FromQuery] string searchTerm = "")
        {
            var doctors = await _doctorProcess.SearchDoctorsAsync(searchTerm);
            return Ok(doctors);
        }
    }
}