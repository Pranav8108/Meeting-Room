namespace IDENTITY.Models
{
    public class BookingModel
    {
        public int ID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string UserID { get; set; } = string.Empty; // this is the ID of the user who booked the room
        public int RoomModelId { get; set; } // this is the ID of the room that was booked

        public RoomModel? RoomModel { get; set; }

		
		//public bool IsBookingConflict(List<BookingModel> existingBookings)
		//{
		//	// Check if there is any conflict with existing bookings
		//	return existingBookings.Any(b =>
		//		b.StartTime <= StartTime && b.EndTime >= StartTime ||
		//		b.StartTime <= EndTime && b.EndTime >= EndTime ||
		//		b.StartTime >= StartTime && b.EndTime <= EndTime);
		//}


	}

}
