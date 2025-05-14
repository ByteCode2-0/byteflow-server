using byteflow_server.Models;
using byteflow_server.Models.DTOs;
using byteflow_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Newtonsoft.Json.Linq;

namespace byteflow_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        //get all attendance
        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAll()
        {
            var attendances = await _attendanceService.GetAllAttendancesAsync();
            return Ok(attendances);
        }

        //get all attendance By user ID  
        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Attendance>> GetById(long id)
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }
            return Ok(attendance);
        }

        //create attendance
        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpPost]
        public async Task<ActionResult<Attendance>> Create(Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _attendanceService.CreateAttendanceAsync(attendance);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Attendance?.AttendanceId }, result.Attendance);
        }

        //review attendance
        [Authorize(Roles = "Admin,Manager,HR")]
        [HttpPatch("{id}/review")]
        public async Task<ActionResult> Review(long id, [FromBody] AttendanceReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _attendanceService.ReviewAttendanceAsync(id, reviewDto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message });
        }

        //update attendance
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] AttendanceCheckoutUpdateDto checkoutDto)
        {
            var existingAttendance = await _attendanceService.GetAttendanceByIdAsync(id);
            if (existingAttendance == null)
                return NotFound();

            if (checkoutDto.CheckOutTime == default)
                return BadRequest("checkOutTime is required and must be a valid date.");

            // Only update the checkout time, preserve all other data
            existingAttendance.CheckOutTime = checkoutDto.CheckOutTime;
            existingAttendance.UpdatedAt = DateTime.UtcNow;

            await _attendanceService.UpdateAttendanceAsync(existingAttendance);
            return NoContent();
        }
        //delete attendance
        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            await _attendanceService.DeleteAttendanceAsync(id);
            return NoContent();
        }

        //get attendance by attendee id
        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpGet("user/{attendeeId}")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetByAttendeeId(long attendeeId)
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value
                   ?? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                   ?? User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                   ?? "";
            long.TryParse(userIdClaim, out var userId);

            // If user is an Employee, they can only view their own attendance records
            if (userRole == "Employee" && userId != attendeeId)
            {
                return Forbid();
            }

            var attendances = await _attendanceService.GetAttendancesByAttendeeIdAsync(attendeeId);
            return Ok(attendances);
        }
    }
}
