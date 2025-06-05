namespace EpamKse.GameStore.DataAccess.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Entities;

public class GameConfiguration : IEntityTypeConfiguration<Game> {
    public void Configure(EntityTypeBuilder<Game> builder) {
        builder.Property(g => g.Id)
            .UseIdentityColumn();
        
        builder.Property(g => g.Title)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(g => g.Description)
            .HasMaxLength(2000);
            
        builder.Property(g => g.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder
            .HasMany(g => g.HistoricalPrices)
            .WithOne(hs => hs.Game);
        builder
            .HasMany(g => g.Platforms)
            .WithMany(p => p.Games)
            .UsingEntity(j => j.ToTable("GamePlatforms"));
        builder
            .HasOne(g => g.Publisher)
            .WithMany(p => p.Games)
            .HasForeignKey(g => g.PublisherId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
