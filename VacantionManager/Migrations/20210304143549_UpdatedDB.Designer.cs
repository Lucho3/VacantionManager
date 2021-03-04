﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VacantionManager.Models;

namespace VacantionManager.Migrations
{
    [DbContext(typeof(VacantionManagerDBContext))]
    [Migration("20210304143549_UpdatedDB")]
    partial class UpdatedDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VacantionManager.Models.Entity.HospitalLeaveModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("ambulatoryCard")
                        .IsRequired()
                        .HasColumnType("image");

                    b.Property<DateTime>("appicationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("date")
                        .HasDefaultValueSql("getdate()");

                    b.Property<int>("applicantid")
                        .HasColumnType("int");

                    b.Property<bool>("approved")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("endDate")
                        .HasColumnType("date");

                    b.Property<DateTime>("startDate")
                        .HasColumnType("date");

                    b.HasKey("id");

                    b.HasIndex("applicantid");

                    b.ToTable("HOspitalLeaves");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.LeaveModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("appicationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("date")
                        .HasDefaultValueSql("getdate()");

                    b.Property<int>("applicantid")
                        .HasColumnType("int");

                    b.Property<bool>("approved")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("endDate")
                        .HasColumnType("date");

                    b.Property<bool>("halfDay")
                        .HasColumnType("bit");

                    b.Property<bool>("isPaid")
                        .HasColumnType("bit");

                    b.Property<DateTime>("startDate")
                        .HasColumnType("date");

                    b.HasKey("id");

                    b.HasIndex("applicantid");

                    b.ToTable("Leaves");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.ProjectModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.RoleModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            id = 1,
                            name = "Unassigned"
                        },
                        new
                        {
                            id = 2,
                            name = "Developer"
                        },
                        new
                        {
                            id = 3,
                            name = "Team Lead"
                        },
                        new
                        {
                            id = 4,
                            name = "CEO"
                        });
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.TeamModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.Property<int?>("projectid")
                        .HasColumnType("int");

                    b.Property<int?>("teamLeaderid")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("projectid");

                    b.HasIndex("teamLeaderid");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.UserModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("firstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("lastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("roleid")
                        .HasColumnType("int");

                    b.Property<int?>("teamid")
                        .HasColumnType("int");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("id");

                    b.HasIndex("roleid");

                    b.HasIndex("teamid");

                    b.HasIndex("username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.HospitalLeaveModel", b =>
                {
                    b.HasOne("VacantionManager.Models.Entity.UserModel", "applicant")
                        .WithMany("hospitalLeaves")
                        .HasForeignKey("applicantid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("applicant");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.LeaveModel", b =>
                {
                    b.HasOne("VacantionManager.Models.Entity.UserModel", "applicant")
                        .WithMany("leaves")
                        .HasForeignKey("applicantid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("applicant");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.TeamModel", b =>
                {
                    b.HasOne("VacantionManager.Models.Entity.ProjectModel", "project")
                        .WithMany("workingTeams")
                        .HasForeignKey("projectid");

                    b.HasOne("VacantionManager.Models.Entity.UserModel", "teamLeader")
                        .WithMany("leadedTeams")
                        .HasForeignKey("teamLeaderid")
                        .HasConstraintName("ForeignKey_Team_Leader");

                    b.Navigation("project");

                    b.Navigation("teamLeader");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.UserModel", b =>
                {
                    b.HasOne("VacantionManager.Models.Entity.RoleModel", "role")
                        .WithMany("users")
                        .HasForeignKey("roleid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VacantionManager.Models.Entity.TeamModel", "team")
                        .WithMany("devs")
                        .HasForeignKey("teamid")
                        .HasConstraintName("ForeignKey_User_Team");

                    b.Navigation("role");

                    b.Navigation("team");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.ProjectModel", b =>
                {
                    b.Navigation("workingTeams");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.RoleModel", b =>
                {
                    b.Navigation("users");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.TeamModel", b =>
                {
                    b.Navigation("devs");
                });

            modelBuilder.Entity("VacantionManager.Models.Entity.UserModel", b =>
                {
                    b.Navigation("hospitalLeaves");

                    b.Navigation("leadedTeams");

                    b.Navigation("leaves");
                });
#pragma warning restore 612, 618
        }
    }
}
