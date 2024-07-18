using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(item => item.Id); 
        builder.Property(item => item.Id)
            .IsRequired()
            .HasConversion(
                id => id.ToString(), // Преобразование Guid в строку при сохранении в базу данных
                id => Guid.Parse(id) // Преобразование строки обратно в Guid при чтении из базы данных
            ); 
        
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
            .IsRequired();

        builder.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // 1-1
        builder
            .HasOne(x => x.Student)
            .WithOne(x => x.User)
            .HasForeignKey<StudentEntity>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);  // Установка каскадного удаления
        
        // 1-1
        builder
            .HasOne(x => x.Teacher)
            .WithOne(x => x.User)
            .HasForeignKey<TeacherEntity>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);  // Установка каскадного удаления
        
        // *-1
        builder
            .HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(item => item.RoleId);
    }
}