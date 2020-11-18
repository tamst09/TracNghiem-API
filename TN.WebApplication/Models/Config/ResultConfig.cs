using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace TN.WebApplication.Models.Config
{
    public class ResultConfig : IEntityTypeConfiguration<Result>
    {
        public void Configure(EntityTypeBuilder<Result> builder)
        {
            builder.ToTable("Results");
            builder.HasKey(r => new { r.UserID, r.QuestionID });
            builder.HasOne(x => x.AppUser).WithMany(u => u.Results).HasForeignKey(x => x.UserID).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Question).WithMany(u => u.Results).HasForeignKey(x => x.QuestionID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
