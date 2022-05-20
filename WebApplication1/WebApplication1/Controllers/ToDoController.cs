using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Service;
using WebApplication1.Models;
using WebApplication1.DTO;

namespace WebApplication1.Controllers
{
  

   
        [Route("api/[controller]")]
        [ApiController]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public class TodoController : ControllerBase
        {
            private readonly ApplicationDbContext _db;
            private readonly IUserService _userService;
            private readonly UserManager<IdentityUser> _userManager;

            public TodoController(
                    ApplicationDbContext db,
                    IUserService userService,
                    UserManager<IdentityUser> userManager
                )
            {
                _db = db;
                _userService = userService;
                _userManager = userManager;
            }

            [HttpGet]
            public async Task<IActionResult> Get()
            {
                string userId = _userService.Id();
                List<Todo> todos = await _db.Todos.Where(t => t.UserId == userId).ToListAsync();
                return Ok(new { data = todos });
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> Get(int id)
            {
                string userId = _userService.Id();
                Todo todo = await _db.Todos.Where(t => t.UserId == userId && t.Id == id).FirstOrDefaultAsync();
                if (todo == null)
                {
                    return NotFound();
                }
                return Ok(new { data = todo });
            }

            [HttpPost]
            public async Task<IActionResult> Post([FromBody] ToDoRequest item)
            {
                string userId = _userService.Id();
                IdentityUser user = await _userManager.FindByIdAsync(userId);
                Todo todo = new Todo() { title = item.title, description = item.description, UserId = userId, User = user };
                await _db.Todos.AddAsync(todo);
                await _db.SaveChangesAsync();
                return CreatedAtAction("Post", new { todo.Id }, todo);

            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Put(int id, [FromBody] ToDoRequest item)
            {
                string userId = _userService.Id();
                IdentityUser user = await _userManager.FindByIdAsync(userId);
                Todo todo = await _db.Todos.Where(t => t.UserId == userId && t.Id == id).FirstOrDefaultAsync();
                if (todo == null) { return NotFound(); }
                if (item.title != null) { todo.title = item.title.Trim(); }
                if (item.description != null) { todo.description = item.description.Trim(); }
                if (item.done != null) { todo.done = item.done; }
                await _db.SaveChangesAsync();
                return Ok("updated successfully");

            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                string userId = _userService.Id();
                IdentityUser user = await _userManager.FindByIdAsync(userId);
                Console.WriteLine("First");
                Todo todo = await _db.Todos.Where(t => t.UserId == userId && t.Id == id).FirstOrDefaultAsync();
                Console.WriteLine("Second");
                if (todo != null)
                {
                    _db.Remove(todo);
                    await _db.SaveChangesAsync();
                    return Ok(new { msg = "removed" });
                }
                return BadRequest("bad request");
            }

        }
    

}
