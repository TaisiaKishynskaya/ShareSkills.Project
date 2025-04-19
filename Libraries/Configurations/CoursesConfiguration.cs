using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class CoursesConfiguration : IEntityTypeConfiguration<CourseEntity>
{
    public void Configure(EntityTypeBuilder<CourseEntity> builder)
    {
        builder.ToTable("Courses");

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
    }
}