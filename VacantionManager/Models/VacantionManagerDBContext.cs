using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacantionManager.Models.Entity;

namespace VacantionManager.Models
{
    public class VacantionManagerDBContext:DbContext
    {
        public VacantionManagerDBContext(DbContextOptions<VacantionManagerDBContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleModel>().HasData(
                new RoleModel { id = 1, name = "Unassigned" },
                new RoleModel { id = 2, name = "Developer" },
                new RoleModel { id = 3, name = "Team Lead" },
                new RoleModel { id = 4, name = "CEO" });

            modelBuilder.Entity<UserModel>().HasIndex(u => u.username).IsUnique();

            modelBuilder.Entity<LeaveModel>()
                            .Property(b => b.appicationDate)
                            .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<LeaveModel>()
                            .Property(b => b.approved)
                            .HasDefaultValue(false);

            modelBuilder.Entity<HospitalLeaveModel>()
                            .Property(b => b.appicationDate)
                            .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<HospitalLeaveModel>()
                            .Property(b => b.approved)
                            .HasDefaultValue(false);

            modelBuilder.Entity<UserModel>()
            .HasOne(p => p.team)
            .WithMany(q => q.devs)
            .HasConstraintName("ForeignKey_User_Team");

            modelBuilder.Entity<TeamModel>()
           .HasOne(p => p.teamLeader)
           .WithMany(q => q.leadedTeams)
           .HasConstraintName("ForeignKey_Team_Leader");

        }
        public virtual DbSet<UserModel> Users { get; set; }

        public virtual DbSet<TeamModel> Teams { get; set; }

        public virtual DbSet<RoleModel> Roles { get; set; }

        public virtual DbSet<ProjectModel> Projects { get; set; }

        public virtual DbSet<LeaveModel> Leaves { get; set; }

        public virtual DbSet<HospitalLeaveModel> HOspitalLeaves { get; set; }
    }
}
