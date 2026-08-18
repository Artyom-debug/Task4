using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task4.Models;

namespace Task4.Data;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.Status)
            .IsRequired()
            .HasDefaultValue(Status.unverified);

        builder.Property(u => u.LastLoginTime)
            .IsRequired();
    }
}
