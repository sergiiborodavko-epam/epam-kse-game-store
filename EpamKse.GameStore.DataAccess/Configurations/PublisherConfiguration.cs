using EpamKse.GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EpamKse.GameStore.DataAccess.Configurations;

public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.ToTable("Publishers");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasColumnName("Name")
            .IsRequired();

        builder.HasIndex(e => e.Name)
            .IsUnique();
        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();
        builder
            .HasMany(p => p.PublisherPlatforms)
            .WithMany(p => p.Publishers)
            .UsingEntity(j => j.ToTable("PublisherPlatforms"));

        builder.HasMany(p => p.Games)
            .WithOne(g => g.Publisher)
            .HasForeignKey(g => g.PublisherId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}