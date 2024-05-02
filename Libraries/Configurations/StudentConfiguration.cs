// using Libraries.Entities.Concrete;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace Libraries.Configurations;
//
// public class StudentConfiguration : IEntityTypeConfiguration<StudentEntity>
// {
//     public void Configure(EntityTypeBuilder<StudentEntity> builder)
//     {
//         builder.ToTable("Students");
//         
//         builder.HasKey(item => item.Id);
//         
//         builder.Property(item => item.Purpose)
//             .IsRequired()
//             .HasMaxLength(200); 
//         
//         // 1-1
//         /*builder
//             .HasOne(x => x.User)
//             .WithOne(x => x.Student)
//             .HasForeignKey<UserEntity>(u => u.StudentId)
//             .OnDelete(DeleteBehavior.Cascade);  // Установка каскадного удаления;*/
//         
//         // *-1
//         builder
//             .HasMany(x => x.Meetings)
//             .WithOne(x => x.Student);
//         
//         // *-*
//         builder
//             .HasMany(x => x.Grades)
//             .WithMany(x => x.Students);
//     }
// }

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
        
        builder.Property(item => item.Purpose)
            .IsRequired()
            .HasMaxLength(200); 
        
        // Связи...
    }
}
