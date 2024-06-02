using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class TimeOfDayConfiguration : IEntityTypeConfiguration<TimeOfDayEntity>
{
    public void Configure(EntityTypeBuilder<TimeOfDayEntity> builder)
    {
        builder.ToTable("TimeOfDays");
        
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
            .IsRequired();
        
        // *-1
        builder
            .HasMany(x => x.Teachers)
            .WithOne(x => x.TimeOfDay);
    }
}
