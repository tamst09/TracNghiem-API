using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TN.WebApplication.Models.Config
{
    public class HistoryConfig : IEntityTypeConfiguration<HistoryExam>
    {
        public void Configure(EntityTypeBuilder<HistoryExam> builder)
        {
            builder.ToTable("HistoryExams");
            builder.HasKey(x => new { x.UserID, x.ExamID });
            builder.HasOne(x => x.AppUser).WithMany(u => u.HistoryExams).HasForeignKey(x => x.UserID).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Exam).WithMany(u => u.HistoryExams).HasForeignKey(x => x.ExamID).OnDelete(DeleteBehavior.NoAction);
            
        }
    }
}
