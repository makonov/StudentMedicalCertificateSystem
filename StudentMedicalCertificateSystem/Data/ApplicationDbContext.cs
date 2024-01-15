using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<StudentGroups> StudentGroups { get; set; }
        public DbSet<EducationalOffices> EducationalOffices { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<Diagnoses> Diagnoses { get; set; }
        public DbSet<Clinics> Clinics { get; set; }
        public DbSet<MedicalCertificates> MedicalCertificates { get; set; }
    }
}
