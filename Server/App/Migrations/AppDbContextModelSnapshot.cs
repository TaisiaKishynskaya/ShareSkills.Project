﻿// <auto-generated />
using System;
using Libraries.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace App.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("GradeEntityStudentEntity", b =>
                {
                    b.Property<Guid>("GradesId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("StudentsId")
                        .HasColumnType("char(36)");

                    b.HasKey("GradesId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("GradeEntityStudentEntity", (string)null);
                });

            modelBuilder.Entity("GradeEntityTeacherEntity", b =>
                {
                    b.Property<Guid>("GradesId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("TeachersId")
                        .HasColumnType("char(36)");

                    b.HasKey("GradesId", "TeachersId");

                    b.HasIndex("TeachersId");

                    b.ToTable("GradeEntityTeacherEntity", (string)null);
                });

            modelBuilder.Entity("Libraries.Contracts.Skill.SkillDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Skill")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("SkillDto", (string)null);
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.ClassTimeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("ClassTimes", (string)null);
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.GradeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Grade")
                        .HasPrecision(1)
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Grades", (string)null);
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.LevelEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Levels", (string)null);
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.MeetingEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<Guid>("ForeignId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("StudentEntityId")
                        .HasColumnType("char(36)");

                    b.Property<string>("UserEntityId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("StudentEntityId");

                    b.HasIndex("UserEntityId");

                    b.ToTable("Meetings", (string)null);
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.RoleEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.SkillEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Skill")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Skills", (string)null);
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.StudentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Purpose")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Students", (string)null);
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.TeacherEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ClassTimeId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("LevelId")
                        .HasColumnType("char(36)");

                    b.Property<double>("Rating")
                        .HasPrecision(2, 1)
                        .HasColumnType("double");

                    b.Property<Guid>("SkillId")
                        .HasColumnType("char(36)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("ClassTimeId");

                    b.HasIndex("LevelId");

                    b.HasIndex("SkillId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Teachers", (string)null);
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.UserEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("SkillEntityUserEntity", b =>
                {
                    b.Property<Guid>("SkillsId")
                        .HasColumnType("char(36)");

                    b.Property<string>("UsersId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("SkillsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("SkillEntityUserEntity", (string)null);
                });

            modelBuilder.Entity("GradeEntityStudentEntity", b =>
                {
                    b.HasOne("Libraries.Entities.Concrete.GradeEntity", null)
                        .WithMany()
                        .HasForeignKey("GradesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Libraries.Entities.Concrete.StudentEntity", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GradeEntityTeacherEntity", b =>
                {
                    b.HasOne("Libraries.Entities.Concrete.GradeEntity", null)
                        .WithMany()
                        .HasForeignKey("GradesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Libraries.Entities.Concrete.TeacherEntity", null)
                        .WithMany()
                        .HasForeignKey("TeachersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.MeetingEntity", b =>
                {
                    b.HasOne("Libraries.Entities.Concrete.StudentEntity", null)
                        .WithMany("Meetings")
                        .HasForeignKey("StudentEntityId");

                    b.HasOne("Libraries.Entities.Concrete.UserEntity", null)
                        .WithMany("Meetings")
                        .HasForeignKey("UserEntityId");
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.StudentEntity", b =>
                {
                    b.HasOne("Libraries.Entities.Concrete.UserEntity", "User")
                        .WithOne("Student")
                        .HasForeignKey("Libraries.Entities.Concrete.StudentEntity", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.TeacherEntity", b =>
                {
                    b.HasOne("Libraries.Entities.Concrete.ClassTimeEntity", "ClassTime")
                        .WithMany("Teachers")
                        .HasForeignKey("ClassTimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Libraries.Entities.Concrete.LevelEntity", "Level")
                        .WithMany("Teachers")
                        .HasForeignKey("LevelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Libraries.Contracts.Skill.SkillDto", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Libraries.Entities.Concrete.UserEntity", "User")
                        .WithOne("Teacher")
                        .HasForeignKey("Libraries.Entities.Concrete.TeacherEntity", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClassTime");

                    b.Navigation("Level");

                    b.Navigation("Skill");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.UserEntity", b =>
                {
                    b.HasOne("Libraries.Entities.Concrete.RoleEntity", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("SkillEntityUserEntity", b =>
                {
                    b.HasOne("Libraries.Entities.Concrete.SkillEntity", null)
                        .WithMany()
                        .HasForeignKey("SkillsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Libraries.Entities.Concrete.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.ClassTimeEntity", b =>
                {
                    b.Navigation("Teachers");
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.LevelEntity", b =>
                {
                    b.Navigation("Teachers");
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.RoleEntity", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.StudentEntity", b =>
                {
                    b.Navigation("Meetings");
                });

            modelBuilder.Entity("Libraries.Entities.Concrete.UserEntity", b =>
                {
                    b.Navigation("Meetings");

                    b.Navigation("Student")
                        .IsRequired();

                    b.Navigation("Teacher")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
