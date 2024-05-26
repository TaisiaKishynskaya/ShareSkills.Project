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
	}
}