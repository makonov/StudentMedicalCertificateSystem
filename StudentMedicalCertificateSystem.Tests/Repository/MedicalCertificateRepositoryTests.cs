using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.Tests.Repository
{
    public class MedicalCertificateRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName : Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.MedicalCertificates.CountAsync() < 0)
            {
                databaseContext.MedicalCertificates.Add(
                    new MedicalCertificate()
                    {
                        Student = new Student()
                        {
                            Group = new StudentGroup()
                            {
                                Program = new EducationalProgram()
                                {
                                    ProgramName = "Программная инженерия"
                                },
                                GroupName = "ПИ-21-1"
                            },
                            LastName = "Игнатова",
                            FirstName = "Жанна",
                            Patronymic = "Павловна",
                            BirthDate = DateTime.ParseExact("16.02.2002", "dd.MM.yyyy", CultureInfo.InvariantCulture)
                        },
                        User = new User()
                        {
                            UserName = "officeWorker",
                            PasswordHash = "HashCode",
                            LastName = "Сахипова",
                            FirstName = "Марина",
                            Patronymic = "Станиславовна"
                        },
                        Clinic = new Clinic()
                        {
                            ClinicName = "Больница №1"
                        },
                        Diagnosis = new Diagnosis()
                        {
                            DiagnosisName = "ОРВИ",
                            Code = "J9.06"
                        },
                        CertificateNumber = 55,
                        CertificatePath = "certificatePath",
                        ClinicAnswerPath = "clinicAnswerPath",
                        IsConfirmed = true,
                        IssueDate = DateTime.ParseExact("16.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                        IllnessDate = DateTime.ParseExact("02.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                        RecoveryDate = DateTime.ParseExact("15.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                        Answer = "нет",
                        CreatedAt = DateTime.ParseExact("17.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                        UpdatedAt = DateTime.ParseExact("17.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture)
                    }) ;
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void MedicalCertificateRepository_Add_ReturnsBool()
        {
            //Arrange
            var certificate = new MedicalCertificate()
            {
                Student = new Student()
                {
                    Group = new StudentGroup()
                    {
                        Program = new EducationalProgram()
                        {
                            ProgramName = "Программная инженерия"
                        },
                        GroupName = "ПИ-21-1"
                    },
                    LastName = "Игнатова",
                    FirstName = "Жанна",
                    Patronymic = "Павловна",
                    BirthDate = DateTime.ParseExact("16.02.2002", "dd.MM.yyyy", CultureInfo.InvariantCulture)
                },
                User = new User()
                {
                    UserName = "officeWorker",
                    PasswordHash = "HashCode",
                    LastName = "Сахипова",
                    FirstName = "Марина",
                    Patronymic = "Станиславовна"
                },
                Clinic = new Clinic()
                {
                    ClinicName = "Больница №1"
                },
                Diagnosis = new Diagnosis()
                {
                    DiagnosisName = "ОРВИ",
                    Code = "J9.06"
                },
                CertificateNumber = 55,
                CertificatePath = "certificatePath",
                ClinicAnswerPath = "clinicAnswerPath",
                IsConfirmed = true,
                IssueDate = DateTime.ParseExact("16.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                IllnessDate = DateTime.ParseExact("02.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                RecoveryDate = DateTime.ParseExact("15.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                Answer = "нет",
                CreatedAt = DateTime.ParseExact("17.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                UpdatedAt = DateTime.ParseExact("17.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };
            var dbContext = await GetDbContext();
            var certificateRepository = new MedicalCertificateRepository(dbContext);

            //Act
            var result = certificateRepository.Add(certificate);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void MedicalCertificateRepository_GetByIdAsync_ReturnsCertificate()
        {
            //Arrange
            var id = 1;
            var dbContext = await GetDbContext();
            var certificateRepository = new MedicalCertificateRepository(dbContext);

            //Act
            var result = certificateRepository.GetByIdAsync(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<MedicalCertificate>>();
        }
    }
}
