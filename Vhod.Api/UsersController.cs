using Microsoft.AspNetCore.Mvc;

namespace Vhod.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly TradeContext _db;
        public UsersController(TradeContext db) { _db = db; }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_db.User.Select(u => new
            {
                u.User_ID,
                u.User_Login,
                u.User_Surname,
                u.User_Name,
                u.User_Patronymic,
                u.User_Role,
                u.User_Status
            }).ToList());
        }

        [HttpPut("{id}/block")]
        public IActionResult Block(int id)
        {
            var u = _db.User.Find(id);
            if (u == null) return NotFound();
            u.User_Status = true;
            _db.SaveChanges();
            return Ok(new { message = "Заблокирован" });
        }

        [HttpPut("{id}/unblock")]
        public IActionResult Unblock(int id)
        {
            var u = _db.User.Find(id);
            if (u == null) return NotFound();
            u.User_Status = false;
            _db.SaveChanges();
            return Ok(new { message = "Разблокирован" });
        }
    }
}