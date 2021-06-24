using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TN.Data.Entities;

namespace TN.Data.Config
{
    public class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.UserName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Name).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(50);
            builder.Property(u => u.PhoneNumber).IsRequired(false).HasMaxLength(50);
            builder.Property(u => u.DoB).IsRequired();
            builder.Property(u => u.PasswordHash).IsRequired(false);
            builder.Property(user => user.RefreshTokenValue).IsRequired(required: false);
            builder.HasOne(u => u.RefreshToken).WithOne(t => t.User).IsRequired(false);
        }
    }
}
