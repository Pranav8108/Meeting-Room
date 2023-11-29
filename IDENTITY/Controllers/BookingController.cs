using IDENTITY.DataAccess;
using IDENTITY.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace IDENTITY.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseContext _databaseContext;

        public BookingController(DatabaseContext databaseContext, UserManager<ApplicationUser> userManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;

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
        public IActionResult BookRoom()
        {
            if (ViewBag.Rooms == null)
            {
                ViewBag.Rooms = _databaseContext.Rooms.ToList();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BookRoom(int RoomModelId, DateTime startTime, DateTime endTime)
        {
            var room = await _databaseContext.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(m => m.Id == RoomModelId);

            if (room == null)
            {
                return NotFound("Room not found ");
            }
            if (room.Status == "NotAvailable")
            {
                return NotFound("Room is unavailable for booking because Room Capacity is Full");
            }
            //
            if (endTime <= startTime)
            {
                return BadRequest("End time must be after the start time.");
            }//

            var userId = User.Identity.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User not found");
            }

            if (ViewBag.Rooms == null)
            {
                ViewBag.Rooms = _databaseContext.Rooms.ToList();
            }

            var booking = new BookingModel
            {
                StartTime = startTime,
                EndTime = endTime,
                UserID = userId,
                RoomModelId = RoomModelId,
            };

            _databaseContext.Bookings.Add(booking);
            await _databaseContext.SaveChangesAsync();

            return RedirectToAction(nameof(BookRoom));
        }


        public async Task<IActionResult> BookingDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _databaseContext.Bookings.Where(m => m.RoomModelId == id).ToListAsync();
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }



        public async Task<IActionResult> GetBookingDetails()
		{
			var booking = await _databaseContext.Bookings.ToListAsync();
			return View(booking);
		}

        public async Task<IActionResult> MyBooking()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            var bookings = await _databaseContext.Bookings
                .Where(p => p.UserID == currentUser.UserName )
                .ToListAsync(); 

            if (bookings == null || !bookings.Any())
            {
                return NotFound("No bookings found for the current user.");
            }

            return View(bookings);
        }



        public async Task<IActionResult> DeleteBooking(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _databaseContext.Bookings.FirstOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);
            if (booking.UserID != currentUser.UserName)
            {
                return RedirectToAction(nameof(Unauthorized));
            }
            return View(booking);
        }
        [HttpPost, ActionName("DeleteBooking")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _databaseContext.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            _databaseContext.Bookings.Remove(booking);
            await _databaseContext.SaveChangesAsync();
            return RedirectToAction(nameof(GetBookingDetails));

        }
        [HttpGet]
        public async Task<IActionResult> EditBooking(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _databaseContext.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (booking.UserID != currentUser.UserName)
            {
                return RedirectToAction(nameof(Unauthorized));
            }

            return View(booking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBooking(int id, BookingModel booking)
        {
            if (id != booking.ID)
            {
                return NotFound();
            }

            var existingBooking = await _databaseContext.Bookings.FindAsync(id);

            if (existingBooking == null)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    existingBooking.StartTime = booking.StartTime;
                    existingBooking.EndTime = booking.EndTime;


                    _databaseContext.Update(existingBooking);
                    await _databaseContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
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
        [HttpGet]
        public IActionResult AddParticipant(int bookingId)
        {
            var booking = _databaseContext.Bookings
                .Include(b => b.RoomModel)
                .FirstOrDefault(b => b.ID == bookingId);
            var room = booking?.RoomModel;
            if (room == null)
            {
                return NotFound();
            }
            var currentParticipantsCount = _databaseContext.Participants
                 .Where(p => p.BookingId == bookingId)
                 .Count();
            if (currentParticipantsCount >= room.Capacity)
            {
                return NotFound("Room is full");
            }

            var currentUser = _userManager.GetUserAsync(User).Result;

            if (booking == null || currentUser.UserName != booking.UserID)
            {
                return RedirectToAction(nameof(Unauthorized));
            }

            var availableParticipants = _userManager.Users.ToList();

            if (availableParticipants == null)
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

                _databaseContext.Participants.Add(participant);
                await _databaseContext.SaveChangesAsync();

                return RedirectToAction("GetBookingDetails", new { id = viewModel.Booking.ID });
            }

            return View(viewModel);
        }

        public async Task<IActionResult> GetParticipants()
        {
            var participants = await _databaseContext.Participants.ToListAsync();
            return View(participants);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveParticipant(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _databaseContext.Participants.FindAsync(id);
            if (participant == null)
            {
                return NotFound();
            }

            var booking = await _databaseContext.Bookings
                .Include(b => b.RoomModel)
                .FirstOrDefaultAsync(b => b.ID == participant.BookingId);

            if (booking == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (booking.UserID != currentUser.UserName)
            {
                return RedirectToAction(nameof(Unauthorized));
            }

            return View(participant);
        }

        [HttpPost, ActionName("RemoveParticipant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveParticipantConfirmed(int id)
        {
            var participant = await _databaseContext.Participants.FindAsync(id);

            if (participant == null)
            {
                return NotFound();
            }

            var booking = await _databaseContext.Bookings
                .FirstOrDefaultAsync(b => b.ID == participant.BookingId);

            if (booking == null)
            {
                return NotFound();
            }



            _databaseContext.Participants.Remove(participant);
            await _databaseContext.SaveChangesAsync();

            return RedirectToAction(nameof(GetParticipants));
        }

        [HttpGet]
        public async Task<IActionResult> ListParticipantWithBookID(int? bookingId)
        {
            if (bookingId == null)
            {
                return NotFound();
            }

            var participants = await _databaseContext.Participants
                .Where(p => p.BookingId == bookingId)
                .ToListAsync();

            if (participants == null || participants.Count == 0)
            {
                return NotFound("There is no participant in this Booked Room");
            }

            return View(participants);
        }

        public IActionResult Unauthorized()
        {
            return View();
        }
        private bool BookingModelExists(int id)
        {
            return _databaseContext.Bookings.Any(e => e.ID == id);
        }


    }
}
