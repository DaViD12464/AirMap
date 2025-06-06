using AirMap.Data;
using Microsoft.EntityFrameworkCore;

namespace AirMap.Helper
{
    /// <summary>
    /// Static helper class for database operations using Entity Framework
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Gets all records from a specific table
        /// </summary>
        /// <typeparam name="TEntity">The entity type of the table</typeparam>
        /// <param name="context">The database context</param>
        /// <returns>A list of all records</returns>
        public static List<TEntity> GetAll<TEntity>(DbContext context) where TEntity : class
        {
            return context.Set<TEntity>().ToList();
        }

        /// <summary>
        /// Gets all records from a specific table asynchronously
        /// </summary>
        /// <typeparam name="TEntity">The entity type of the table</typeparam>
        /// <param name="context">The database context</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all records</returns>
        public static async Task<List<TEntity>> GetAllAsync<TEntity>(DbContext context) where TEntity : class
        {
            return await context.Set<TEntity>().ToListAsync();
        }
    }
}
