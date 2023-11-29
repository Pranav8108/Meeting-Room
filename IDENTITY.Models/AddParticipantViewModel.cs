namespace IDENTITY.Models
{
    public class AddParticipantViewModel
    {
        public BookingModel? Booking { get; set; }
        public string? UserId { get; set; }
        public List<ApplicationUser>? AvailableParticipants { get; set; }
    }
}
