using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace TN.WebApplication.Models.Config
{
    public class ExamConfig : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.ToTable("Exams");
            builder.HasKey(e => e.ID);
            builder.HasOne(e => e.Category).WithMany(c => c.Exams).HasForeignKey(e => e.CategoryID);
            builder.HasMany(e => e.Questions).WithOne(q => q.Exam);
            builder.HasOne(e => e.AppUser).WithMany(a => a.Exams).HasForeignKey(e => e.UserID);
            builder.Property(e => e.ExamName).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Time).IsRequired(false);
            builder.Property(e => e.NumOfAttemps).HasDefaultValue(0);
            builder.Property(e => e.CategoryID).IsRequired();
            builder.Property(e => e.TimeCreated).IsRequired();
        }
    }
}
