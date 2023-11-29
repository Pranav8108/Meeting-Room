using IDENTITY.Models;

namespace IDENTITY.Repositories.Abstract
{
	public interface IParticipantRepository : IGenericRepository<ParticipantModel>
	{
		void Update(ParticipantModel model);
	}
}
