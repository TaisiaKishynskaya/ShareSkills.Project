using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<TeacherEntity>
{
    public void Configure(EntityTypeBuilder<TeacherEntity> builder)
    {
        builder.ToTable("Teachers");
        
        builder.HasKey(item => item.Id);
        
        builder.Property(item => item.Rating)
            .IsRequired()
            .HasPrecision(2, 1); // Максимальне значення рейтинга буде мати 5 цифр, з яких 2 - після крапки; 
        
        // *-*
        builder
            .HasMany(x => x.Grades)
            .WithMany(x => x.Teachers);
    }
}