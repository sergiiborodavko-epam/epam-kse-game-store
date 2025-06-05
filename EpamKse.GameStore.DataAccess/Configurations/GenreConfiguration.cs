namespace EpamKse.GameStore.DataAccess.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Entities;

public class GenreConfiguration : IEntityTypeConfiguration<Genre> {
    public void Configure(EntityTypeBuilder<Genre> builder) {
        builder.Property(g => g.Id)
            .UseIdentityColumn();
            
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(g => g.ParentGenre)
            .WithMany(g => g.SubGenres)
            .HasForeignKey(g => g.ParentGenreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
