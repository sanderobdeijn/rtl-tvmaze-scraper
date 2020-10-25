using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rtl.TvMaze.Scraper
{
    public interface IScraperNode
    {
        Task<IEnumerable<ShowDto>> GetShowsForPage(int page);

        Task<IEnumerable<ActorDto>> GetActorsForShow(int tvMazeId);
    }
}