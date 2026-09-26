using Microsoft.AspNetCore.Mvc;
using Vhod.Models;

namespace Vhod.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TradeContext _db;
        public AuthController(TradeContext db) { _db = db; }

        public class LoginRequest
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var user = _db.User.FirstOrDefault(
                u => u.User_Login == req.Login && u.User_Password == req.Password);

            if (user == null)
                return Unauthorized(new { message = "Неверный логин или пароль" });

            if (user.User_Status)
                return StatusCode(403, new { message = "Вы заблокированы" });

            return Ok(new
            {
                user.User_ID,
                user.User_Login,
                user.User_Role,
                user.User_Surname,
                user.User_Name
            });
        }

        public class RegisterRequest
        {
            public string Surname { get; set; }
            public string Name { get; set; }
            public string Patronymic { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            if (_db.User.Any(u => u.User_Login == req.Login))
                return BadRequest(new { message = "Логин занят" });

            var user = new User
            {
                User_Surname = req.Surname,
                User_Name = req.Name,
                User_Patronymic = req.Patronymic,
                User_Login = req.Login,
                User_Password = req.Password,
                User_Role = 2,
                User_Status = false
            };
            _db.User.Add(user);
            _db.SaveChanges();
            return Ok(new { message = "OK", user.User_ID });
        }
    }
}