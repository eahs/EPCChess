
using Microsoft.EntityFrameworkCore;

namespace ADSBackend.Data
{
    /// <summary>
    /// Provides extension methods for the <see cref="ApplicationDbContext"/>.
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// Adds or updates an entity in the context based on its state.
        /// </summary>
        /// <param name="ctx">The database context.</param>
        /// <param name="entity">The entity to add or update.</param>
        public static void AddOrUpdate(this ApplicationDbContext ctx, object entity)
        {
            var entry = ctx.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Detached:
                    ctx.Add(entity);
                    break;
                case EntityState.Modified:
                    ctx.Update(entity);
                    break;
                case EntityState.Added:
                    ctx.Add(entity);
                    break;
                case EntityState.Unchanged:
                    // item already in db no need to do anything  
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}