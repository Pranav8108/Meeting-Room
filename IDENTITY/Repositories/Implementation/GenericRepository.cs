using IDENTITY.DataAccess;
using IDENTITY.Repositories.Abstract;
using Microsoft.Build.Framework;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace IDENTITY.Repositories.Implementation
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly DatabaseContext _db;
		internal DbSet<T> dbSet;
        public GenericRepository(DatabaseContext db)
        {
			_db = db;
		     this.dbSet = _db.Set<T>();
		}

		public void Add(T model)
		{
			dbSet.Add(model);
			_db.SaveChanges();
		}

		public async void AddAsync(T model)
		{
			await dbSet.AddAsync(model);
		}

		public void Delete(T model)
		{
			dbSet.Remove(model);
			_db.SaveChanges();
		}

	

		public IEnumerable<T> GetAll()
		{
			return dbSet.ToList();
		}

		public T GetById(int id)
		{
			return dbSet.Find(id);
		}

		public T GetFirstorDefault(Expression<Func<T, bool>> filter)
		{
			return dbSet.FirstOrDefault(filter);
		}

		public void RemoveRange(IEnumerable<T> model)
		{
			dbSet.RemoveRange(model);
		}

		public void Update(T model)
		{
			dbSet.Update(model);
			_db.SaveChanges();
		}
	}
}
