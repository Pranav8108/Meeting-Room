using IDENTITY.DataAccess;
using IDENTITY.Repositories.Abstract;

namespace IDENTITY.Repositories.Implementation
{
	public class UnitOfWork:IUnitOfWork
	{
		private readonly DatabaseContext _dbContext;
		public UnitOfWork(DatabaseContext dbContext)
		{
			_dbContext = dbContext;
			RoomRepository = new RoomRepository(_dbContext);
			BookingRepository = new BookingRepository(_dbContext);
			ParticipantRepository = new ParticipantRepository(_dbContext);
		}
		public IRoomRepository RoomRepository { get; private set; }

		public IBookingRepository BookingRepository { get; private set; }
		public IParticipantRepository ParticipantRepository { get; private set; }

		public int save()
		{
			return _dbContext.SaveChanges();
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}
	}
}
