using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<SkillEntity>
{
    public void Configure(EntityTypeBuilder<SkillEntity> builder)
    {
        builder.ToTable("Skills");
        
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Skill)
            .IsRequired()
            .HasMaxLength(100);
        
        // *-*
        builder
            .HasMany(x => x.Users)
            .WithMany(x => x.Skills);
    }
}