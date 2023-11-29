
using IDENTITY.DataAccess;
using IDENTITY.Models;
using IDENTITY.Repositories.Abstract;

using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace IDENTITY.Repositories.Implementation
{
	public class RoomRepository : GenericRepository<RoomModel>, IRoomRepository
	{
		private readonly DatabaseContext _databaseContext;
		public RoomRepository(DatabaseContext databaseContext) : base(databaseContext)
		{
			_databaseContext = databaseContext;
		}

		public async Task CreateRoomAsync(RoomModel room)
		{
			await _databaseContext.Rooms.AddAsync(room);
			await _databaseContext.SaveChangesAsync();
		}
	


		public async Task DeleteRoomAsync(int? id)
		{
			var room = await _databaseContext.Rooms.FindAsync(id);
			if (room != null)
			{
				_databaseContext.Rooms.Remove(room);
				await _databaseContext.SaveChangesAsync();
			}
		}
		public async Task EditRoomAsync(RoomModel room)
		{
			_databaseContext.Rooms.Update(room);
			await _databaseContext.SaveChangesAsync();
		}

		public async Task<RoomModel> GetRoomDetailsAsync(int? id)
		{
			if (id == null)
			{
				return null;
			}

			var room = await _databaseContext.Rooms.Where(r => r.Id == id).FirstOrDefaultAsync();
			return room;
			//return await _databaseContext.Rooms.FirstOrDefaultAsync(r => r.Id == id);
		}
		}
		
	
}

