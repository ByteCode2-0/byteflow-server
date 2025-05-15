using byteflow_server.Models;
using byteflow_server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
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
       
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchLeaveRequest(long id, [FromBody] JsonPatchDocument<LeaveRequest> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Invalid patch document");
            }

            // Retrieve the leave request from the service
            var leaveRequest = await _leaveService.GetLeaveRequestByIdAsync(id);
            if (leaveRequest == null)
            {
                return NotFound("Leave request not found");
            }

            // Apply the patch to the leave request
            patchDoc.ApplyTo(leaveRequest, (error) =>
            {
                ModelState.AddModelError(error.AffectedObject.ToString(), error.ErrorMessage);
            });

            // Validate the patched object
            if (!TryValidateModel(leaveRequest))
            {
                return BadRequest(ModelState);
            }

            // Update the leave request in the database
            await _leaveService.UpdateLeaveRequestAsync(leaveRequest);

            //return NoContent();
            return Ok(leaveRequest);
        }

        [HttpGet("reviewer/{id}")]
        public async Task<IActionResult> GetAllLeaveRequestsReview(long id)
        {
            var reviews = await _leaveService.GetAllLeaveRequestsReviewByAsync(id);
            if (reviews == null)
            {
                return NotFound("Reviews not found for the given leave request ID");
            }
            return Ok(reviews);
        }


        [HttpGet("employee/{id}")]
        public async Task<IActionResult> GetLeaveRequestsByEmployeeId(long id)
        {
            var employee = await _leaveService.GetLeaveRequestsByEmployeeIdAsync(id);
            if(employee == null)
            {
                return NotFound("Employee not found for the leave request id");
            }
            return Ok(employee);
        }
    }
}
