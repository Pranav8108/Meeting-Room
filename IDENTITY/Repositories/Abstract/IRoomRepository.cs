using IDENTITY.Models;

namespace IDENTITY.Repositories.Abstract
{
	public interface IRoomRepository:IGenericRepository<RoomModel>
	{
		Task<RoomModel> GetRoomDetailsAsync(int? id);
		Task  CreateRoomAsync (RoomModel room);
		Task EditRoomAsync(RoomModel room);
		Task DeleteRoomAsync(int? id);

		



	}

}
