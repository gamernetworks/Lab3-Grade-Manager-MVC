using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Lab3GradeM.Models;

namespace Lab3GradeM.Data
{
    public partial class LAB3GMContext : DbContext
    {
        public LAB3GMContext()
        {
        }

        public LAB3GMContext(DbContextOptions<LAB3GMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<Classroom> Classrooms { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=gamer1;Initial Catalog=LAB3GM;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_Assignments_Students");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("FK_Students_Classrooms");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
