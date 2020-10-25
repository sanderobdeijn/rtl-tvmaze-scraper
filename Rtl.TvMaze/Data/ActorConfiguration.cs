using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rtl.TvMaze.Models;

namespace Rtl.TvMaze.Data
{
    public class ActorConfiguration : IEntityTypeConfiguration<Actor>
    {
        public void Configure(EntityTypeBuilder<Actor> builder)
        {
            builder
                .Property(s => s.Id)
                .ValueGeneratedNever()
                .IsRequired();
        }
    }
}