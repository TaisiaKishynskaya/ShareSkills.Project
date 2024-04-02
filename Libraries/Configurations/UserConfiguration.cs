using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        //builder.ToTable("User");
        
        builder.HasKey(item => item.Id); 
        
        builder.Property(item => item.Name)
            .IsRequired()
            .HasMaxLength(30); 
        
        builder.Property(item => item.Surname)
            .IsRequired()
            .HasMaxLength(30); 
        
        builder.Property(item => item.Email)
            .IsRequired()
            .HasMaxLength(50); 
        
        builder.Property(item => item.Role)
            .IsRequired()
            .HasMaxLength(7); 
        
        builder.Property(item => item.Password)
            .IsRequired()
            .HasMaxLength(15); 
        
        builder.Property(item => item.Objective)
            .HasMaxLength(300); 
        
        // 1-1
        builder
            .HasOne(x => x.Student)
            .WithOne(x => x.User);
        
        // 1-1
        builder
            .HasOne(x => x.Teacher)
            .WithOne(x => x.User);
    }
}