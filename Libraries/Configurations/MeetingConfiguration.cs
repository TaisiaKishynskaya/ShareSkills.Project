using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<MeetingEntity>
{
    public void Configure(EntityTypeBuilder<MeetingEntity> builder)
    {
        // builder.ToTable("Meeting");
        
        builder.HasKey(item => item.Id);
        
        builder.Property(b => b.DateAndTime).IsRequired();
        
        // *-1
        builder
            .HasOne(x => x.Teacher)
            .WithMany(x => x.Meetings)
            .HasForeignKey(item => item.TeacherId);
        
        // *-1
        builder
            .HasOne(x => x.Student)
            .WithMany(x => x.Meetings)
            .HasForeignKey(item => item.StudentId);
    }
}