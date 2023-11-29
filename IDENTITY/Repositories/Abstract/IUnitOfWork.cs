using IDENTITY.DataAccess;
using IDENTITY.Models;
using IDENTITY.Repositories.Implementation;

namespace IDENTITY.Repositories.Abstract
{
	public interface IUnitOfWork:IDisposable
	{
		IRoomRepository RoomRepository { get; }
		IBookingRepository BookingRepository { get; }
		IParticipantRepository ParticipantRepository { get; }
		int save();
    }
}
