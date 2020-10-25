using System.Collections.Generic;

namespace Rtl.TvMaze.Shows
{
    public class ShowViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<CastViewModel> Cast { get; set; }
    }
}