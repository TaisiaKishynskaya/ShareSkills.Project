using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<StudentEntity>
{
    public void Configure(EntityTypeBuilder<StudentEntity> builder)
    {
        builder.ToTable("Students");
        
        builder.HasKey(item => item.Id);
        
        builder.Property(item => item.Purpose)
            .IsRequired()
            .HasMaxLength(200); 
        
        // *-*
        builder
            .HasMany(x => x.Grades)
            .WithMany(x => x.Students);
    }
}