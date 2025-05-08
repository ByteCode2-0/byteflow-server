using byteflow_server.Models;
using byteflow_server.Models.DTOs;
using byteflow_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace byteflow_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [Authorize(Roles = "Admin,Manager,Hr,Employee")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetEmployeeByUserId(long userId)
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var employee = employees.FirstOrDefault(e => e.UserId == userId);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee); 
        }




        //[Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        //[Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee is null) 
            {
                return NotFound("Employee not found.");
            }
            return Ok(employee);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _employeeService.CreateEmployeeWithUserAsync(createDto);
            
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Employee.EmployeeId }, result.Employee);
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Employee updatedEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound("Employee not found.");
            }

            updatedEmployee.EmployeeId = id;
            await _employeeService.UpdateEmployeeAsync(updatedEmployee);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            await _employeeService.DeleteEmployeeAsync(id);
            return NoContent(); 
        }
       
    }
}
