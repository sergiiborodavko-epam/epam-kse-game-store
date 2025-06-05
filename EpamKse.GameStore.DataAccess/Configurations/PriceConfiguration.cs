using EpamKse.GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EpamKse.GameStore.DataAccess.Configurations;

public class PriceConfiguration: IEntityTypeConfiguration<HistoricalPrice>
{
    public void Configure(EntityTypeBuilder<HistoricalPrice> builder)
    {
        builder.ToTable("HistoricalPrices");
        builder.HasKey(e => e.Id);
        builder.Property(p => p.Id)
            .UseIdentityColumn();

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.HasOne(h => h.Game)
            .WithMany(g => g.HistoricalPrices)
            .HasForeignKey(h => h.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
