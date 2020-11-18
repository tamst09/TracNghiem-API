using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TN.Data.Entities;

namespace TN.Data.Config
{
    public class ResultConfig : IEntityTypeConfiguration<Result>
    {
        public void Configure(EntityTypeBuilder<Result> builder)
        {
            builder.ToTable("Results");
            builder.HasKey(r => new { r.UserID, r.QuestionID });
            builder.HasOne(x => x.AppUser).WithMany().HasForeignKey(x => x.UserID).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Question).WithMany(x=>x.Results).HasForeignKey(x => x.QuestionID).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
