using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TN.Data.Entities;

namespace TN.Data.DataContext.DbSeeding
{
    public static class ContextExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            const int Admin_ID = 1;
            const int User_ID = 2;
            modelBuilder.Entity<AppRole>().HasData(
                new AppRole
                {
                    Id = Admin_ID,
                    Name = "admin",
                    NormalizedName = "ADMIN",
                    Description = "Administrator level"
                },
                new AppRole
                {
                    Id = User_ID,
                    Name = "user",
                    NormalizedName = "USER",
                    Description = "User level"
                });

            var hasher = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = Admin_ID,
                    UserName = "admin1999",
                    NormalizedUserName = "admin1999",
                    Email = "tamst09@gmail.com",
                    NormalizedEmail = "TAMST09@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Admin@123"),
                    SecurityStamp = string.Empty,
                    FirstName = "Admin",
                    LastName = "Default",
                    DoB = new DateTime(1999, 4, 26),
                    PhoneNumber = "0336709707",
                    isActive = true
                },
                new AppUser
                {
                    Id = User_ID,
                    UserName = "user1999",
                    NormalizedUserName = "USER1999",
                    Email = "thtt260499@gmail.com",
                    NormalizedEmail = "THTT260499@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "User@123"),
                    SecurityStamp = string.Empty,
                    FirstName = "User",
                    LastName = "Default",
                    DoB = new DateTime(1999, 4, 26),
                    PhoneNumber = "0336709707",
                    isActive = true
                }
            );
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(
                new IdentityUserRole<int>
                {
                    RoleId = Admin_ID,
                    UserId = Admin_ID
                },
                new IdentityUserRole<int>
                {
                    RoleId = User_ID,
                    UserId = User_ID
                }
            );
        }
    }
}
