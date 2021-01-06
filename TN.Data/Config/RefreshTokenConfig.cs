using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TN.Data.Entities;

namespace TN.Data.Config
{
    public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(e => e.Token);
            builder.HasOne(e => e.User).WithOne(c => c.RefreshToken).HasForeignKey<AppUser>(u => u.RefreshTokenValue).IsRequired(false);
            builder.Property(e => e.ExpiryDate).IsRequired();
            builder.Property(e => e.Token).IsRequired();
        }
    }
}
