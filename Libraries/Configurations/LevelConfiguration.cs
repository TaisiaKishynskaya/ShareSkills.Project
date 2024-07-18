using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class LevelConfiguration : IEntityTypeConfiguration<LevelEntity>
{
    public void Configure(EntityTypeBuilder<LevelEntity> builder)
    {
        builder.ToTable("Levels");
        
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
            .IsRequired();
        
        // *-1
        builder
            .HasMany(x => x.Teachers)
            .WithOne(x => x.Level);
    }
}