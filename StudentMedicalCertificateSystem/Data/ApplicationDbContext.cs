using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // Запрещаем каскадное удаление
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentGroup>()
                .HasOne(g => g.Program)
                .WithMany()
                .HasForeignKey(g => g.ProgramID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany()
                .HasForeignKey(s => s.GroupID)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<MedicalCertificate>()
                .HasOne(c => c.Diagnosis)
                .WithMany()
                .HasForeignKey(c => c.DiagnosisID)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<MedicalCertificate>()
                .HasOne(c => c.Clinic)
                .WithMany()
                .HasForeignKey(c => c.ClinicID)
                .OnDelete(DeleteBehavior.Restrict); 

        }

        public DbSet<User> Users { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<EducationalProgram> EducationalPrograms { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<MedicalCertificate> MedicalCertificates { get; set; }
    }
}
