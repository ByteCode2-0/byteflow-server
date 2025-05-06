using byteflow_server.Models;
using byteflow_server.Services;
using Microsoft.AspNetCore.Mvc;

namespace byteflow_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLeaveRequests()
        {
            var leaveRequests = await _leaveService.GetAllLeaveRequestsAsync();
            return Ok(leaveRequests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveRequestById(long id)
        {
            var leaveRequest = await _leaveService.GetLeaveRequestByIdAsync(id);
            if (leaveRequest == null)
            {
                return NotFound("Leave request not found");
            }
            return Ok(leaveRequest);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] LeaveRequest leaveRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _leaveService.CreateLeaveRequestAsync(leaveRequest);
            return CreatedAtAction(nameof(GetLeaveRequestById), new { id = leaveRequest.LeaveRequestId }, leaveRequest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveRequest(long id, [FromBody] LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.LeaveRequestId)
            {
                return BadRequest("Leave request ID mismatch");
            }

            await _leaveService.UpdateLeaveRequestAsync(leaveRequest);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(long id)
        {
            await _leaveService.DeleteLeaveRequestAsync(id);
            return NoContent();
        }
    }
}
