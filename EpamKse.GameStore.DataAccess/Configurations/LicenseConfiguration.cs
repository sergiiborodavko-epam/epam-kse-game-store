using EpamKse.GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EpamKse.GameStore.DataAccess.Configurations;

public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.ToTable("Licenses");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(l => l.Key)
            .IsRequired();

        builder.Property(l => l.OrderId)
            .IsRequired();
        
        builder.HasOne(l => l.Order)
            .WithOne(o => o.License)
            .HasForeignKey<License>(l => l.OrderId);
    }
}