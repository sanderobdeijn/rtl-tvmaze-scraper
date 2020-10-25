using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rtl.TvMaze.Data;
using Rtl.TvMaze.Models;

namespace Rtl.TvMaze.Shows
{
    public class ShowsService : IShowsService
    {
        private readonly TvMazeDbContext tvMazeDbContext;

        public ShowsService(TvMazeDbContext tvMazeDbContext)
        {
            this.tvMazeDbContext = tvMazeDbContext;
        }
        public IEnumerable<ShowViewModel> GetShows(int page, int pageSize)
        {
            var shows = tvMazeDbContext.Set<Show>()
                .Include(s => s.ShowActors)
                    .ThenInclude(sp => sp.Actor)
                .OrderBy(s => s.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(show => new ShowViewModel
                {
                    Id = show.Id,
                    Name = show.Name,
                    Cast = show.ShowActors
                        .Select(c => c.Actor)
                        .OrderByDescending(a => a.DateOfBirth)
                        .Select(a =>
                            new CastViewModel
                            {
                                Id = a.Id,
                                Name = a.Name,
                                BirthDay = a.DateOfBirth
                            })
                });

            return shows;
        }
    }
}