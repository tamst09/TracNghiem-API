using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TN.WebApplication.Models;

namespace TN.WebApplication.Data.Seeding
{
    public static class ContextExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            const int Admin_ID = 1;
            const int User_ID = 2;
            List<AppRole> appRoles = new List<AppRole>();
            appRoles.Add(new AppRole { Id = User_ID, Name = "user", NormalizedName = "USER", Description = "User level" });
            appRoles.Add(new AppRole { Id = Admin_ID, Name = "admin", NormalizedName = "ADMIN", Description = "Administrator level" });
            modelBuilder.Entity<AppRole>().HasData(appRoles);

            var hasher = new PasswordHasher<AppUser>();
            List<AppUser> appUsers = new List<AppUser>();
            appUsers.Add(new AppUser
            {
                Id = User_ID,
                UserName = "user1999",
                NormalizedUserName = "USER1999",
                Email = "thtt260499@gmail.com",
                NormalizedEmail = "THTT260499@GMAIL.COM",
                EmailConfirmed = false,
                PasswordHash = hasher.HashPassword(null, "User@123"),
                SecurityStamp = string.Empty,
                FirstName = "User",
                LastName = "Default",
                DoB = new DateTime(1999, 4, 26)
            });
            appUsers.Add(new AppUser
            {
                Id = Admin_ID,
                UserName = "admin1999",
                NormalizedUserName = "admin1999",
                Email = "tamst09@gmail.com",
                NormalizedEmail = "tamst09@gmail.com",
                EmailConfirmed = false,
                PasswordHash = hasher.HashPassword(null, "Admin@123"),
                SecurityStamp = string.Empty,
                FirstName = "Admin",
                LastName = "Default",
                DoB = new DateTime(1999, 4, 26)
            });
            modelBuilder.Entity<AppUser>().HasData(appUsers);
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
            });
        }
    }
}
