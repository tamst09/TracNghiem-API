using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TN.Data.Entities;

namespace TN.Data.Config
{
    public class ExamConfig : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.ToTable("Exams");
            builder.HasKey(e => e.ID);
            builder.HasOne(e => e.Category).WithMany(c => c.Exams).HasForeignKey(e => e.CategoryID).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
            builder.HasOne(e => e.Owner).WithMany().HasForeignKey(e => e.OwnerID).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.Questions).WithOne(q => q.Exam);
            builder.Property(e => e.ExamName).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Time).IsRequired(false);
            builder.Property(e => e.OwnerID).IsRequired();
            builder.Property(e => e.NumOfAttemps).HasDefaultValue(0);
            builder.Property(e => e.CategoryID).IsRequired(false);
            builder.Property(e => e.TimeCreated).IsRequired();
        }
    }
}
