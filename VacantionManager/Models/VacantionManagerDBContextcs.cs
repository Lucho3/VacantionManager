using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacantionManager.Models.Entity;

namespace VacantionManager.Models
{
    public class VacantionManagerDBContextcs:DbContext
    {
        public VacantionManagerDBContextcs(DbContextOptions<VacantionManagerDBContextcs> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .Property(b => b.role)
                .HasDefaultValue(4);

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

        }
        public virtual DbSet<UserModel> Users { get; set; }

        public virtual DbSet<TeamModel> Teams { get; set; }

        public virtual DbSet<RoleModel> Roles { get; set; }

        public virtual DbSet<ProjectModel> Projects { get; set; }

        public virtual DbSet<LeaveModel> Leaves { get; set; }

        public virtual DbSet<HospitalLeaveModel> HOspitalLeaves { get; set; }
    }
}
