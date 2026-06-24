using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebServices.Controllers.Scheduling
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoctorScheduleController : ControllerBase
    {
        public IActionResult GetDoctorsScheduleByDate([FromBody] DateOnly date)
        {
            return Ok();
        }
    }
}
