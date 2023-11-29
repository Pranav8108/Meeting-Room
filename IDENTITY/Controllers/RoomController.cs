using IDENTITY.DataAccess;
using IDENTITY.Models;
using IDENTITY.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IDENTITY.Controllers
{
    public class RoomController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IUnitOfWork _unitOfWork;
		

		public RoomController(DatabaseContext databaseContext, IUnitOfWork _unitOfWork)
        {
            _databaseContext = databaseContext;
			this._unitOfWork = _unitOfWork;
		}

        public IActionResult Index()
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
        public IActionResult CreateRoom()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoom([Bind("Id,RoomName,Capacity")] RoomModel room)//k kura chainca tyo mara use garcha Bind ley
        {

            if (ModelState.IsValid)
            {
                _databaseContext.Add(room);
                await _databaseContext.SaveChangesAsync();
                return RedirectToAction(nameof(GetRoom));
            }
            return View(room);
        }

    }
}
