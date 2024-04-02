using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<StudentEntity>
{
    public void Configure(EntityTypeBuilder<StudentEntity> builder)
    {
        // builder.ToTable("Student");
        
        builder.HasKey(item => item.Id);
        
        builder.Property(item => item.Purpose)
            .IsRequired()
            .HasMaxLength(200); 
        
        // 1-1
        builder
            .HasOne(x => x.User)
            .WithOne(x => x.Student);
        
        // *-1
        builder
            .HasMany(x => x.Meetings)
            .WithOne(x => x.Student);
        
        // *-*
        builder
            .HasMany(x => x.Grades)
            .WithMany(x => x.Students);
    }
}