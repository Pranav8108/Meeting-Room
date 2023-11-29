using IDENTITY.DataAccess;
using IDENTITY.Models;
using IDENTITY.Repositories.Abstract;

namespace IDENTITY.Repositories.Implementation
{
	public class ParticipantRepository : GenericRepository<ParticipantModel>, IParticipantRepository
	{
		private readonly DatabaseContext _databaseContext;
        public ParticipantRepository(DatabaseContext databaseContext) : base(databaseContext) 
        {
            _databaseContext = databaseContext;
        }
        public void Update(ParticipantModel participant)
        {
            _databaseContext.Participants.Update(participant);
        }

    }
}
