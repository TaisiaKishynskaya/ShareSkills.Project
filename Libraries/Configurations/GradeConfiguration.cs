using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class GradeConfiguration : IEntityTypeConfiguration<GradeEntity>
{
    public void Configure(EntityTypeBuilder<GradeEntity> builder)
    {
        builder.ToTable("Grades");
        
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Grade)
            .HasPrecision(1, 0); // максимальна оцінка 5, тобто складається з 1 цифри з якої 0 цифр після коми
        
        // *-*
        builder
            .HasMany(x => x.Teachers)
            .WithMany(x => x.Grades);
        
        // *-*
        builder
            .HasMany(x => x.Students)
            .WithMany(x => x.Grades);
    }
}