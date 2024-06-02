using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class ClassTimeConfiguration : IEntityTypeConfiguration<ClassTimeEntity>
{
    public void Configure(EntityTypeBuilder<ClassTimeEntity> builder)
    {
        builder.ToTable("ClassTimes");
        
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
            .IsRequired();
        
        // *-1
        builder
            .HasMany(x => x.Teachers)
            .WithOne(x => x.ClassTime);
    }
}
