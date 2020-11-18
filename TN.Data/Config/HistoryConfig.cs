using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TN.Data.Entities;

namespace TN.Data.Config
{
    public class HistoryConfig : IEntityTypeConfiguration<HistoryExam>
    {
        public void Configure(EntityTypeBuilder<HistoryExam> builder)
        {
            builder.ToTable("HistoryExams");
            builder.HasKey(x => new { x.UserID, x.ExamID });
            builder.HasOne(x => x.AppUser).WithMany().HasForeignKey(x => x.UserID).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Exam).WithMany().HasForeignKey(x => x.ExamID).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
