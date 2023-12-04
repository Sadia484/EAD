using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.DbModels
{
    public class StudentDbContext : DbContext
    {
        public DbSet<StudentDbDto> Students { get; set; }
        public DbSet<StudentSubjectDbDto> StudentSubjects { get; set; }
        public DbSet<SubjectDbDto> Subjects { get; set; }

        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary key for StudentDbDto
            modelBuilder.Entity<StudentDbDto>().HasKey(e => e.Id);

            // Configure one-to-many relationship between StudentDbDto and StudentSubjectDbDto
            modelBuilder.Entity<StudentDbDto>()
                .HasMany(s => s.studentSubjects)
                .WithOne(ss => ss.studentDbDto)
                .HasForeignKey(ss => ss.SID)
                .OnDelete(DeleteBehavior.Cascade); // Adjust the deletion behavior according to your needs

            // Configure primary key for SubjectDbDto
            modelBuilder.Entity<SubjectDbDto>().HasKey(e => e.id);

            // Configure one-to-many relationship between SubjectDbDto and StudentSubjectDbDto
            modelBuilder.Entity<SubjectDbDto>()
                .HasMany(sub => sub.StudentSubjects)
                .WithOne(ss => ss.SubjectDbDto)
                .HasForeignKey(ss => ss.SubjectId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust the deletion behavior according to your needs
        }
    }
}