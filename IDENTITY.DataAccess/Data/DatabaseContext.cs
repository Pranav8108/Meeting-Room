using IDENTITY.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IDENTITY.DataAccess
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options ) : base(options) 
        
        {
                
        }
        public DbSet <ApplicationUser> ApplicationUser { get; set; }

        public DbSet<RoomModel> Rooms { get; set; }

        public DbSet<BookingModel > Bookings { get; set; }

        public DbSet<ParticipantModel> Participants { get; set; }

		public object Where(Func<object, bool> value)
		{
			throw new NotImplementedException();
		}
	}
}
