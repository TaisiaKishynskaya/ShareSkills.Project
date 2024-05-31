using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Roles");
        
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
            .IsRequired();
        
        // *-1
        builder
            .HasMany(x => x.Users)
            .WithOne(x => x.Role);
    }
}