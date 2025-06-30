namespace EpamKse.GameStore.DataAccess.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Entities;

public class GameCountryBanConfiguration : IEntityTypeConfiguration<GameBan> {
    public void Configure(EntityTypeBuilder<GameBan> builder) {
        builder.ToTable("GameCountryBans");
        
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Id)
            .UseIdentityColumn();
            
        builder.Property(b => b.Country)
            .HasConversion<string>()
            .IsRequired();
            
        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();
            
        builder.HasOne(b => b.Game)
            .WithMany(g => g.GameBans)
            .HasForeignKey(b => b.GameId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(b => new { b.GameId, b.Country })
            .IsUnique();
    }
}
