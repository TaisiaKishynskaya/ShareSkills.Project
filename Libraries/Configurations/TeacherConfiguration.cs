// using Libraries.Entities.Concrete;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace Libraries.Configurations;
//
// public class TeacherConfiguration : IEntityTypeConfiguration<TeacherEntity>
// {
//     public void Configure(EntityTypeBuilder<TeacherEntity> builder)
//     {
//         builder.ToTable("Teachers");
//         
//         builder.HasKey(item => item.Id);
//         
//         builder.Property(item => item.Rating)
//             .IsRequired()
//             .HasPrecision(2, 1); // Максимальне значення рейтинга буде мати 5 цифр, з яких 2 - після крапки; 
//         
//         // 1-1
//         /*builder
//             .HasOne(x => x.User)
//             .WithOne(x => x.Teacher)
//             .HasForeignKey<UserEntity>(s => s.TeacherId)
//             .OnDelete(DeleteBehavior.Cascade);  // Установка каскадного удаления*/
//         
//         // *-1
//         builder
//             .HasMany(x => x.Meetings)
//             .WithOne(x => x.Teacher);
//         
//         // *-*
//         builder
//             .HasMany(x => x.Grades)
//             .WithMany(x => x.Teachers);
//     }
// }

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
        
        // Поля из UserEntity
        builder.Property(item => item.Name)
            .IsRequired()
            .HasMaxLength(30); 
        
        builder.Property(item => item.Surname)
            .IsRequired()
            .HasMaxLength(30); 
        
        builder.Property(item => item.Email)
            .IsRequired()
            .HasMaxLength(50); 
        
        builder.Property(item => item.Password)
            .IsRequired()
            .HasMaxLength(15); 
        
        builder.Property(item => item.Rating)
            .IsRequired()
            .HasPrecision(2, 1); // Максимальне значення рейтинга буде мати 5 цифр, з яких 2 - після крапки; 
        
        // Связи...
    }
}
    