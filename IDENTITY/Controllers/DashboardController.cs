using IDENTITY.DataAccess;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IDENTITY.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        public DashboardController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public IActionResult Display()
        {
            return View();
        }
        public async Task<IActionResult> GetRoom()
        {
            var rooms = await _databaseContext.Rooms.ToListAsync();
            return View(rooms);
        }
        public async Task<IActionResult> GetRoomDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var room = await _databaseContext.Rooms.FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }
    }
}
