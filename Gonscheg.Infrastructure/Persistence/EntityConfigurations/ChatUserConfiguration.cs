using Gonscheg.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gonscheg.Infrastructure.Persistence.EntityConfigurations;

public class ChatUserConfiguration : IEntityTypeConfiguration<ChatUser>
{
    public void Configure(EntityTypeBuilder<ChatUser> builder)
    {
        builder.Property(p => p.BirthDate).HasColumnType("timestamp without time zone");
    }
}