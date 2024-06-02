using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Libraries.Configurations;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}
	
	public DbSet<UserEntity> Users { get; set; }
	public DbSet<StudentEntity> Students { get; set; }
	public DbSet<TeacherEntity> Teachers { get; set; }
	public DbSet<MeetingEntity> Meetings { get; set; }
	public DbSet<GradeEntity> Grades { get; set; }
	public DbSet<SkillEntity> Skills { get; set; }
	public DbSet<RoleEntity> Roles { get; set; }
	public DbSet<TimeOfDayEntity> TimeOfDays { get; set; }
	public DbSet<GoalEntity> Goals { get; set; }
	
	protected override void OnModelCreating(ModelBuilder builder)
	{
		//base.OnModelCreating(builder);
		builder.ApplyConfiguration(new UserConfiguration());
		builder.ApplyConfiguration(new StudentConfiguration());
		builder.ApplyConfiguration(new TeacherConfiguration());
		builder.ApplyConfiguration(new MeetingConfiguration());
		builder.ApplyConfiguration(new GradeConfiguration());
		builder.ApplyConfiguration(new SkillConfiguration());
		builder.ApplyConfiguration(new RoleConfiguration());
		builder.ApplyConfiguration(new TimeOfDayConfiguration());
		builder.ApplyConfiguration(new GoalConfiguration());
	}
	
	/* не получилась валидация имейла.
	 public override int SaveChanges()
	{
		foreach (var entry in ChangeTracker.Entries<UserEntity>())
		{
			if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
			{
				if (!entry.Entity.Email.Contains("@"))
				{
					throw new ValidationException("Пошта повинна містити '@'.");
				}
			}
		}

		return base.SaveChanges();
	}*/
}