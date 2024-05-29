using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<MeetingEntity>
{
    public void Configure(EntityTypeBuilder<MeetingEntity> builder)
    {
        builder.ToTable("Meetings");
        
        builder.HasKey(item => item.Id);

        builder.Property(x => x.OwnerId).IsRequired();
        builder.Property(x => x.Description);
        builder.Property(x => x.ForeignId).IsRequired();
        builder.Property(x => x.DateTime).IsRequired();
    }
}