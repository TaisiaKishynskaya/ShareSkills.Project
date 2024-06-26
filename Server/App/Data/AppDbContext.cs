﻿using App.Data;
using Libraries.Configurations;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

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
	
	protected override void OnModelCreating(ModelBuilder builder)
	{
		//builder.ApplyConfigurationsFromAssembly(GetType().Assembly); // it for Configurations in App project

		builder.ApplyConfiguration(new UserConfiguration());
		builder.ApplyConfiguration(new StudentConfiguration());
		builder.ApplyConfiguration(new TeacherConfiguration());
		builder.ApplyConfiguration(new MeetingConfiguration());
		builder.ApplyConfiguration(new GradeConfiguration());
		
		//AppDbSeed.Seed(builder);
	}
}
