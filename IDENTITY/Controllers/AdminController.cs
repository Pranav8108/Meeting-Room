using IDENTITY.DataAccess;
using IDENTITY.Models;
using IDENTITY.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace IDENTITY.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private readonly DatabaseContext _databasecontext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public IActionResult Display()
        {
            return View();
        }

        public AdminController(UserManager<ApplicationUser> userManager , IUnitOfWork _unitOfWork, DatabaseContext databaseContext)
        {
            _userManager = userManager;
           
			this._unitOfWork = _unitOfWork;
            _databasecontext = databaseContext;
        }

        public  IActionResult GetRoom()
        {
            var rooms = _unitOfWork.RoomRepository.GetAll();
           

            return View(rooms);
        }
        
        public async Task<IActionResult> GetRoomDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var room = await _unitOfWork.RoomRepository.GetRoomDetailsAsync(id);
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
        public async Task<IActionResult> CreateRoom(RoomModel room)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.RoomRepository.CreateRoomAsync(room);
                
                return RedirectToAction(nameof(GetRoom));           
            }
            return View(room);
        }

        public async Task<IActionResult> EditRoom(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var room = await _unitOfWork.RoomRepository.GetRoomDetailsAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }
        [HttpPost]
        public async Task <IActionResult>EditRoom(int id, RoomModel room)
        {
            if(id!  == room.Id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    await _unitOfWork.RoomRepository.EditRoomAsync(room);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomModelExists(room.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }
                return RedirectToAction(nameof(GetRoom));
            }
            return View(room);
        }
        private bool RoomModelExists(int id)
        {
            return _unitOfWork.RoomRepository.GetFirstorDefault(r => r.Id == id) == null;
        }

        public async Task<IActionResult>DeleteRoom(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var room = await _unitOfWork.RoomRepository.GetRoomDetailsAsync(id); 
            if(room == null)
            {
                return NotFound();
            }
            return View(room);

        }
        [HttpPost, ActionName("DeleteRoom")]
        public async Task<IActionResult>DeleteConfirmed(int id)
        {
            await  _unitOfWork.RoomRepository.DeleteRoomAsync(id);
            //var room = await _databasecontext.Rooms.FindAsync(id);
            //_databasecontext.Rooms.Remove(room);
            //await _databasecontext.SaveChangesAsync();
            return RedirectToAction(nameof(GetRoom));
        }
        public IActionResult BookRoom()
        {
            if (ViewBag.Rooms == null) //viewbag passes data from controller to the view
            {
                //ViewBag.Rooms = _databasecontext.Rooms.ToList();
                ViewBag.Rooms = _unitOfWork.RoomRepository.GetAll();
            }
            return View();
        }
		//[HttpPost]
		//public async Task<IActionResult>BookRoom(int RoomModelId, DateTime startTime, DateTime endTime)
		//{
		//    //var room = await _databasecontext.Rooms.
		//    //                Include(r => r.Bookings)
		//    //                .FirstOrDefaultAsync(m => m.Id == RoomModelId);

		//    //if (room.Bookings.Any(b => b.StartTime <= startTime && b.EndTime >= startTime) || room.Bookings.Any(b => b.StartTime <= endTime && b.EndTime >= endTime) || room.Bookings.Any(b => b.StartTime >= startTime && b.EndTime <= endTime))
		//    //{
		//    //    return NotFound("Room is already booked");
		//    //}
		//    //if (room == null)
		//    //{
		//    //    return NotFound("Room Not Found");
		//    //}

		//    var userId = User.Identity.Name; //currently logged in user ko ho bhanera
		//    if(string.IsNullOrEmpty(userId))
		//    {
		//        return NotFound("User not found");
		//    }
		//    if (ViewBag.Rooms == null)
		//    {
		//        ViewBag.Rooms = _unitOfWork.BookingRepository.GetAll();
		//    }
		//    //var booking = new BookingModel
		//    //{
		//    //    StartTime = startTime,
		//    //    EndTime = endTime,
		//    //    UserID = userId,  // Assign the user's ID
		//    //    RoomModelId = RoomModelId,
		//    //};
		//    //         if (room.Bookings.Any(b => b.IsBookingConflict(new List<BookingModel> { booking })))
		//    //{
		//    //	return NotFound("Room is already booked during the selected time slot");
		//    //}
		//    //_databasecontext.Bookings.Add(booking);
		//    //         await _databasecontext.SaveChangesAsync();
		//    //         return RedirectToAction(nameof(GetBookingDetails));
		//    await _unitOfWork.BookingRepository.BookRoomAsync(RoomModelId,  startTime, endTime);


		//}
		[HttpPost]
		public async Task<IActionResult> BookRoom(int RoomModelId, DateTime startTime, DateTime endTime)
		{
			var userId = User.Identity.Name; 
			if (string.IsNullOrEmpty(userId))
			{
				return NotFound("User not found");
			}

			if (ViewBag.Rooms == null)
			{
				ViewBag.Rooms = _unitOfWork.BookingRepository.GetAll();
			}

			var bookingResult = await _unitOfWork.BookingRepository.BookRoomAsync(RoomModelId, userId, startTime, endTime);

			if (bookingResult)
			{
				// Booking was successful
				return RedirectToAction(nameof(GetBookingDetails));
			}
			else
			{
				// Handle the case where booking failed (room not found or conflict)
				return NotFound("Room is already booked during the selected time slot");
			}
		}

		public async Task<IActionResult> GetBookingDetails()
        {
            var booking = await _databasecontext.Bookings.ToListAsync();
            return View(booking);
        }
        public async Task<IActionResult> BookingDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _databasecontext.Bookings.Where(m => m.RoomModelId == id).ToListAsync();
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }
       
        public async Task<IActionResult> DeleteBooking(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _databasecontext.Bookings.FirstOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }
        [HttpPost, ActionName("DeleteBooking")]
        
        public async Task<IActionResult> DeleteBookingConfirmed(int id)
        {

            var booking = await _databasecontext.Bookings.FindAsync(id);
            _databasecontext.Bookings.Remove(booking);
            await _databasecontext.SaveChangesAsync();
            return RedirectToAction(nameof(GetBookingDetails));
        }
        public async Task<IActionResult>EditBooking(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var booking = await _databasecontext.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>EditBooking(int id,BookingModel booking)
        {
            if(id != booking.ID)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    _databasecontext.Bookings.Update(booking);
                    await _databasecontext.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if (!BookingModelExists(booking.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetBookingDetails));
            }
            return View(booking);
        }
        private bool BookingModelExists(int id)
        {
            return _databasecontext.Bookings.Any(e => e.ID == id);
        }

        public IActionResult AddParticipant(int bookingId)
        {
            var booking = _databasecontext.Bookings
                .Include(b => b.RoomModel)
                .FirstOrDefault(b => b.ID == bookingId);

            var room = booking?.RoomModel;

            if (room == null)
            {
                return NotFound();
            }

            var currentParticipantsCount = _databasecontext.Participants
                .Where(p => p.BookingId == bookingId)
                .Count();

            if (currentParticipantsCount >= room.Capacity)
            {
                return NotFound("Room is full");
            }
            var availableParticipants = _userManager.Users.Where(u => u != null).ToList();
             
            if(availableParticipants == null) 
            {
                availableParticipants = new List<ApplicationUser>();
            }

            var viewModel = new AddParticipantViewModel
            {
                Booking = booking,
                AvailableParticipants = availableParticipants
            };

            ViewBag.AvailableParticipants = availableParticipants;

            return View(viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> AddParticipant(AddParticipantViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                var participant = new ParticipantModel
                {
                    UserId = viewModel.UserId,
                    BookingId = viewModel.Booking.ID
                };

                _databasecontext.Participants.Add(participant);
                await _databasecontext.SaveChangesAsync();

                return RedirectToAction("GetParticipants", new { id = viewModel.Booking.ID });
            }

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> ListParticipantWithBookID(int? bookingId)
        {
            if (bookingId == null)
            {
                return NotFound();
            }

            var participants = await _databasecontext.Participants
                .Where(p => p.BookingId == bookingId)
                .ToListAsync();

            if (participants == null || participants.Count == 0)
            {
                return NotFound("There is no participant in this Booked Room");
            }

            return View(participants);
        }

        public async Task<IActionResult> GetParticipants()
        {
            var participants = await _databasecontext.Participants.ToListAsync();
            return View(participants);
        }
        public async Task<IActionResult> RemoveParticipant(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var participant = await _databasecontext.Participants.FirstOrDefaultAsync(m => m.Id == id);
            if (participant == null)
            {
                return NotFound();
            }
            return View(participant);
        }
        [HttpPost, ActionName("RemoveParticipant")]
        //  [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveParticipantConfirmed(int id)
        {

            var participant = await _databasecontext.Participants.FindAsync(id);
            _databasecontext.Participants.Remove(participant);
            await _databasecontext.SaveChangesAsync();
            return RedirectToAction(nameof(GetParticipants));
        }



    }
}
