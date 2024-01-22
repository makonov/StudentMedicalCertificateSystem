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

        public DbSet<User> Users { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<EducationalOffice> EducationalOffices { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<MedicalCertificate> MedicalCertificates { get; set; }
    }
}
