using byteflow_server.DataAccess;
using byteflow_server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace byteflow_server.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        private readonly ByteFlowDbContext context;
        public UserController(ByteFlowDbContext _context)
        {
            context = _context;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> getAll()
        {
            var users = context.Users.ToList();

            await Task.Delay(1000);
            return Ok(users);
        }
    }
}
