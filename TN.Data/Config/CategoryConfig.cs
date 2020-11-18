using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TN.Data.Entities;

namespace TN.Data.Config
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(c => c.ID);
            builder.HasMany(c => c.Exams).WithOne(e => e.Category);
            builder.Property(c => c.ID).UseIdentityColumn();
            builder.Property(c => c.CategoryName).HasMaxLength(100);
            builder.Property(c => c.CategoryName).IsRequired();
        }
    }
}
