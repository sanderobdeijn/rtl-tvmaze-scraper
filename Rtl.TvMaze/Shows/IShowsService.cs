using System.Collections.Generic;

namespace Rtl.TvMaze.Shows
{
    public interface IShowsService
    {
        IEnumerable<ShowViewModel> GetShows(int page, int pageSize);
    }
}