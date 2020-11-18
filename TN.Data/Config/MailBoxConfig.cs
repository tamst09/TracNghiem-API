using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TN.Data.Entities;

namespace TN.Data.Config
{
    public class MailBoxConfig : IEntityTypeConfiguration<MailBox>
    {
        public void Configure(EntityTypeBuilder<MailBox> builder)
        {
            builder.ToTable("Mailboxes");
            builder.HasKey(r => r.ID);
            builder.Property(r => r.ID).UseIdentityColumn();
        }
    }
}
