using IDENTITY.DataAccess;
using IDENTITY.Models;
using IDENTITY.Repositories.Abstract;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace IDENTITY.Repositories.Implementation
{
	public class BookingRepository:GenericRepository<BookingModel>,IBookingRepository
	{
		private readonly DatabaseContext _databaseContext;
        public BookingRepository(DatabaseContext databaseContext) : base(databaseContext)
        {
            _databaseContext = databaseContext;
        }

		//public async Task BookRoomAsync(BookingModel model)
		//{
		//	var room = await _databaseContext.Rooms.
		//				  Include(r => r.Bookings)
		//				  .FirstOrDefaultAsync(m => m.Id == RoomModelId);
		//	if (room == null)
		//	{
		//		return NotFound("Room Not Found");
		//	}

		//	_databaseContext.Bookings.Update(model);
		//	_databaseContext.SaveChangesAsync();
		//	return Task.CompletedTask;

		//}

		public async Task<bool> BookRoomAsync(int RoomModelId, string userId, DateTime startTime, DateTime endTime)
		{
			var room = await _databaseContext.Rooms
				.Include(r => r.Bookings)
				.FirstOrDefaultAsync(m => m.Id == RoomModelId);
			
			if (room == null)
			{
				return false; // Room not found
			}
			if (room.Bookings.Any(b => b.StartTime <= startTime && b.EndTime >= startTime) || room.Bookings.Any(b => b.StartTime <= endTime && b.EndTime >= endTime) || room.Bookings.Any(b => b.StartTime >= startTime && b.EndTime <= endTime))

			{
				return false;
			}

			var booking = new BookingModel
			{
				StartTime = startTime,
				EndTime = endTime,
				UserID = userId,
				RoomModelId = RoomModelId,
			};

			//if (room.Bookings.Any(b => b.IsBookingConflict(new List<BookingModel> { booking })))
			//{
			//	return false; // Conflict, room already booked during the selected time slot
			//}

			// If no conflict and room exists, save the booking
			_databaseContext.Bookings.Add(booking);
			await _databaseContext.SaveChangesAsync();

			return true; // Booking successful
		}


		public Task DeleteBookingAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task EditBookingAsync(BookingModel model)
		{
			throw new NotImplementedException();
		}

		public async Task<BookingModel> GetBookingDetailsAsync(int? id)
		{
			if (id == null)
			{
				return null;
			}
			return await _databaseContext.Bookings.Where(r => r.ID == id).FirstOrDefaultAsync();
		}
	}
}
