using System.Collections.Generic;

namespace Rtl.TvMaze.Models
{
    public class Show
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ShowActor> ShowActors { get; set; }
    }
}