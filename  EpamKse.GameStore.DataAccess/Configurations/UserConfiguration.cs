using EpamKse.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EpamKse.GameStore.DataAccess.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
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
