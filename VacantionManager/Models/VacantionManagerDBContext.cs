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

            modelBuilder.Entity<TeamModel>().HasIndex(u => u.name).IsUnique();

            modelBuilder.Entity<ProjectModel>().HasIndex(u => u.name).IsUnique();

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

            modelBuilder.Entity<TeamModel>().HasIndex(u => u.teamLeaderId).IsUnique();

            modelBuilder.Entity<UserModel>()
                .HasOne(u => u.leadedTeam).WithOne(t => t.teamLeader).HasForeignKey<TeamModel>(t => t.teamLeaderId).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TeamModel>().Property(t => t.teamLeaderId).IsRequired(false);

            //Cascade

            modelBuilder.Entity<RoleModel>().HasMany(r => r.users).WithOne(r => r.role).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProjectModel>().HasMany(t=>t.workingTeams).WithOne(r => r.project).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TeamModel>().HasMany(t => t.devs).WithOne(u=>u.team).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<LeaveModel>().HasOne(u => u.applicant).WithMany(l=>l.leaves).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HospitalLeaveModel>().HasOne(u => u.applicant).WithMany(l => l.hospitalLeaves).OnDelete(DeleteBehavior.Cascade);

        }
        public virtual DbSet<UserModel> Users { get; set; }

        public virtual DbSet<TeamModel> Teams { get; set; }

        public virtual DbSet<RoleModel> Roles { get; set; }

        public virtual DbSet<ProjectModel> Projects { get; set; }

        public virtual DbSet<LeaveModel> Leaves { get; set; }

        public virtual DbSet<HospitalLeaveModel> HospitalLeaves { get; set; }

    }
}
