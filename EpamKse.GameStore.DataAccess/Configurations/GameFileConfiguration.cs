namespace EpamKse.GameStore.DataAccess.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

public class GameFileConfiguration : IEntityTypeConfiguration<GameFile> {
    public void Configure(EntityTypeBuilder<GameFile> builder) {
        builder.ToTable("GameFiles");
        
        builder.HasKey(gf => gf.Id);
        
        builder.Property(gf => gf.Id)
            .UseIdentityColumn();
            
        builder.Property(gf => gf.FileName)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(gf => gf.FilePath)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(gf => gf.FileExtension)
            .IsRequired()
            .HasMaxLength(10);
            
        builder.Property(gf => gf.FileSize)
            .IsRequired();
            
        builder.Property(gf => gf.UploadedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();
            
        builder.Property(gf => gf.GameId)
            .IsRequired();
            
        builder.Property(gf => gf.PlatformId)
            .IsRequired();
            
        builder.HasIndex(gf => new { gf.GameId, gf.PlatformId })
            .IsUnique();
    }
}
