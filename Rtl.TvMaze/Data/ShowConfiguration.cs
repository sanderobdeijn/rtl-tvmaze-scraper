using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rtl.TvMaze.Models;

namespace Rtl.TvMaze.Data
{
    public class ShowConfiguration : IEntityTypeConfiguration<Show>
    {
        public void Configure(EntityTypeBuilder<Show> builder)
        {
            builder
                .Property(s => s.Id)
                .ValueGeneratedNever()
                .IsRequired();
        }
    }
}