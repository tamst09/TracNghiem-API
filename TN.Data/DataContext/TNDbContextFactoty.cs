using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TN.Data.DataContext
{
    public class TNDbContextFactoty : IDesignTimeDbContextFactory<TNDbContext>
    {
        public TNDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connString = configuration.GetConnectionString("TNDatabase");

            var optionsBuilder = new DbContextOptionsBuilder<TNDbContext>();
            optionsBuilder.UseSqlServer(connString);

            return new TNDbContext(optionsBuilder.Options);
        }
    }
}
