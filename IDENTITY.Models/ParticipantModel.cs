using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDENTITY.Models
{
    public class ParticipantModel
    {
        public int Id { get; set; }
        [Required]
        [ForeignKey("BookingModel")]
        public int? BookingId { get; set; }
        [Required]
        [ForeignKey("ApplicationUser")]

        public string? UserId { get; set; }

    }
}
