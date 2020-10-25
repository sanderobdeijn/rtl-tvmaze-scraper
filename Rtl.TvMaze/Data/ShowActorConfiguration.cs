using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rtl.TvMaze.Models;

namespace Rtl.TvMaze.Data
{
    public class ShowActorConfiguration : IEntityTypeConfiguration<ShowActor>
    {
        public void Configure(EntityTypeBuilder<ShowActor> builder)
        {
            builder.HasKey(c => new { c.ShowId, c.ActorId });
        }
    }
}