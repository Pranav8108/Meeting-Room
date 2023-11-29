using System.Linq.Expressions;

namespace IDENTITY.Repositories.Abstract
{
	public interface IGenericRepository<T> where T: class
	{
		T GetFirstorDefault(Expression<Func<T, bool>> filter);
		
		void RemoveRange(IEnumerable<T> model);
		void Add(T model);
		void AddAsync(T model);
		void Update(T model);
		
		T GetById(int id);
		IEnumerable<T> GetAll();
		void Delete(T model);
	}
}
