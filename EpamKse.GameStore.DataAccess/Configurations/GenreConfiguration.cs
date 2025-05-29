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
    }
}
