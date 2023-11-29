using IDENTITY.Models;

namespace IDENTITY.Repositories.Abstract
{
	public interface IBookingRepository :IGenericRepository<BookingModel>
	{
		Task<bool> BookRoomAsync(int RoomModelId,string userId, DateTime startTime, DateTime endTime);
		Task<BookingModel> GetBookingDetailsAsync (int? id);
		Task DeleteBookingAsync (int id);
		Task EditBookingAsync (BookingModel model);
	}
	
}
