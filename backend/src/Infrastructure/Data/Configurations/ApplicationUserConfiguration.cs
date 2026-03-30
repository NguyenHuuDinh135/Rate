using backend.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.Address)
            .HasMaxLength(300);

        builder.Property(u => u.Contact)
            .HasMaxLength(50);

        builder.Property(u => u.CreatedAt)
            .IsRequired();
    }
}