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
        
        // 1-1
        builder
            .HasOne(x => x.User)
            .WithOne(x => x.Teacher)
            .HasForeignKey<UserEntity>(s => s.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);  // Установка каскадного удаления
        
        // *-1
        builder
            .HasMany(x => x.Meetings)
            .WithOne(x => x.Teacher);
        
        // *-*
        builder
            .HasMany(x => x.Grades)
            .WithMany(x => x.Teachers);
    }
}