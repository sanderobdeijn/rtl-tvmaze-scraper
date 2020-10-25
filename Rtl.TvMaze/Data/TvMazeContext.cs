using Microsoft.EntityFrameworkCore;
using Rtl.TvMaze.Models;

namespace Rtl.TvMaze.Data
{
    public class TvMazeDbContext : DbContext
    {
        public TvMazeDbContext(DbContextOptions<TvMazeDbContext> options) :
            base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(Actor).Assembly);
        }
    }
}