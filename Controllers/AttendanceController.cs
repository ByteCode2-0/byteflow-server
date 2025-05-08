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

        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAll()
        {
            var attendances = await _attendanceService.GetAllAttendancesAsync();
            return Ok(attendances);
        }

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

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("{id}/review")]
        public async Task<ActionResult> Review(long id, AttendanceReviewDto reviewDto)
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

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] object body)
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value
                   ?? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                   ?? User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                   ?? "";
            long.TryParse(userIdClaim, out var userId);

            var existingAttendance = await _attendanceService.GetAttendanceByIdAsync(id);
            if (existingAttendance == null)
                return NotFound();

            if (userRole == "Employee")
            {
                // Only allow updating CheckOutTime for their own record
                if (existingAttendance.AttendeeId != userId)
                    return Forbid();

                // Deserialize to DTO
                var checkoutDto = Newtonsoft.Json.JsonConvert.DeserializeObject<AttendanceCheckoutUpdateDto>(body.ToString());
                if (checkoutDto == null || checkoutDto.CheckOutTime == default)
                    return BadRequest("checkOutTime is required and must be a valid date.");

                existingAttendance.CheckOutTime = checkoutDto.CheckOutTime;
                await _attendanceService.UpdateAttendanceAsync(existingAttendance);
                return NoContent();
            }
            else if (userRole == "Admin" || userRole == "Manager" || userRole == "HR")
            {
                // Allow full update
                var updatedAttendance = Newtonsoft.Json.JsonConvert.DeserializeObject<Attendance>(body.ToString());
                if (updatedAttendance == null)
                    return BadRequest("Invalid attendance data.");

                updatedAttendance.AttendanceId = id;
                await _attendanceService.UpdateAttendanceAsync(updatedAttendance);
                return NoContent();
            }
            else
            {
                return Forbid();
            }
        }

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
