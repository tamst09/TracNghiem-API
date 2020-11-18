using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace TN.WebApplication.Models.Config
{
    public class QuestionConfig : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");
            builder.HasKey(q => q.ID);
            builder.HasOne(q => q.Exam).WithMany(e => e.Questions).HasForeignKey(q => q.ExamID);
            builder.Property(q => q.ID).UseIdentityColumn();
            builder.Property(q => q.QuesContent).IsRequired();
            builder.Property(q => q.Option1).IsRequired();
            builder.Property(q => q.Option2).IsRequired();
            builder.Property(q => q.Option3).IsRequired(false);
            builder.Property(q => q.Option4).IsRequired(false);
            builder.Property(q => q.ImgURL).IsRequired(false);
            builder.Property(q => q.Answer).IsRequired();
            builder.Property(q => q.STT).IsRequired();
            builder.Property(q => q.ExamID).IsRequired();
        }
    }
}
