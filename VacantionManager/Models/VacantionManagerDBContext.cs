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

            modelBuilder.Entity<UserModel>()
               .Property(b => b.id)
               .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<RoleModel>()
               .Property(b => b.id)
               .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<TeamModel>()
               .Property(b => b.id)
               .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<ProjectModel>()
               .Property(b => b.id)
               .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<LeaveModel>()
               .Property(b => b.id)
               .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<HospitalLeaveModel>()
               .Property(b => b.id)
               .ValueGeneratedOnAddOrUpdate();

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

            //Conections
            modelBuilder.Entity<UserModel>()
           .HasOne<RoleModel>()
           .WithMany()
           .HasForeignKey(p => p.id)
           .HasConstraintName("ForeignKey_User_Role");

            modelBuilder.Entity<UserModel>()
           .HasOne<TeamModel>()
           .WithMany()
           .HasForeignKey(p => p.id)
           .HasConstraintName("ForeignKey_User_Team");

            modelBuilder.Entity<LeaveModel>()
           .HasOne<UserModel>()
           .WithMany()
           .HasForeignKey(p => p.id)
           .HasConstraintName("ForeignKey_Leave_User");

            modelBuilder.Entity<HospitalLeaveModel>()
           .HasOne<UserModel>()
           .WithMany()
           .HasForeignKey(p => p.id)
           .HasConstraintName("ForeignKey_LeaveHospital_User");

            modelBuilder.Entity<TeamModel>()
           .HasOne<UserModel>()
           .WithMany()
           .HasForeignKey(p => p.id)
           .HasConstraintName("ForeignKey_Team_User");

            modelBuilder.Entity<TeamModel>()
           .HasOne<ProjectModel>()
           .WithMany()
           .HasForeignKey(p => p.id)
           .HasConstraintName("ForeignKey_Team_Project");

        }
        public virtual DbSet<UserModel> Users { get; set; }

        public virtual DbSet<TeamModel> Teams { get; set; }

        public virtual DbSet<RoleModel> Roles { get; set; }

        public virtual DbSet<ProjectModel> Projects { get; set; }

        public virtual DbSet<LeaveModel> Leaves { get; set; }

        public virtual DbSet<HospitalLeaveModel> HOspitalLeaves { get; set; }
    }
}
