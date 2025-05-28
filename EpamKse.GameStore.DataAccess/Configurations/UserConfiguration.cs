namespace EpamKse.GameStore.DataAccess.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Domain.Entities;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(c => c.Id)
            .UseIdentityColumn();
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .IsRequired();
        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();
    }
}