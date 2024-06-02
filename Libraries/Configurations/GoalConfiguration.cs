using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libraries.Configurations;

public class GoalConfiguration : IEntityTypeConfiguration<GoalEntity>
{
    public void Configure(EntityTypeBuilder<GoalEntity> builder)
    {
        builder.ToTable("Goals");
        
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
            .IsRequired();
        
        // *-1
        builder
            .HasMany(x => x.Teachers)
            .WithOne(x => x.Goal);
    }
}