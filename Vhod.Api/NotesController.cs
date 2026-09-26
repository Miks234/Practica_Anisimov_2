using Microsoft.AspNetCore.Mvc;

namespace Vhod.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly TradeContext _db;
        public NotesController(TradeContext db) { _db = db; }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var list = _db.notes
                    .Join(_db.User, n => n.Id_user, u => u.User_ID,
                          (n, u) => new
                          {
                              id = n.Id,
                              title_user = n.Title + " - " + u.User_Login,
                              content = n.Content,
                              formatted_date = n.created_at.ToString("dd.MM.yyyy")
                          })
                    .ToList();
                return Ok(list);
            }
            catch
            {
                return StatusCode(500, new { error = "Ошибка подключения к базе данных" });
            }
        }
    }
}