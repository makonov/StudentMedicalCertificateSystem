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
            var addedCertificate = await dbContext.MedicalCertificates.FindAsync(certificate.CertificateID);
            addedCertificate.Should().NotBeNull();
            addedCertificate.Should().BeOfType<MedicalCertificate>();
        }

        [Fact]
        public async void MedicalCertificateRepository_Delete_ReturnsBool()
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
            certificateRepository.Add(certificate);
            var result = certificateRepository.Delete(certificate);


            //Assert
            result.Should().BeTrue();
            var deletedCertificate = await dbContext.MedicalCertificates.FindAsync(certificate.CertificateID);
            deletedCertificate.Should().BeNull();
        }

        [Fact]
        public async void MedicalCertificateRepository_Update_ReturnsBool()
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
            certificateRepository.Add(certificate);
            certificate.CertificateNumber = 50;
            var result = certificateRepository.Update(certificate);


            //Assert
            result.Should().BeTrue();
            var updatedCertificate = await dbContext.MedicalCertificates.FindAsync(certificate.CertificateID);
            updatedCertificate.CertificateNumber.Should().Be(50);
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

        private List<MedicalCertificate> GetListOfMedicalCertificates()
        {
            var certificate1 = new MedicalCertificate()
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
            var certificate2 = new MedicalCertificate()
            {
                Student = new Student()
                {
                    Group = new StudentGroup()
                    {
                        Program = certificate1.Student.Group.Program,
                        GroupName = "ПИ-21-1"
                    },
                    LastName = "Иванов",
                    FirstName = "Иван",
                    Patronymic = "Иванович",
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
            var certificate3 = new MedicalCertificate()
            {
                Student = new Student()
                {
                    Group = new StudentGroup()
                    {
                        Program = new EducationalProgram()
                        {
                            ProgramName = "Бизнес-информатика"
                        },
                        GroupName = "БИ-21-1"
                    },
                    LastName = "Петров",
                    FirstName = "Петр",
                    Patronymic = "Петрович",
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
            var certificate4 = new MedicalCertificate()
            {
                Student = certificate1.Student,
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
                IssueDate = DateTime.ParseExact("16.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                IllnessDate = DateTime.ParseExact("02.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                RecoveryDate = DateTime.ParseExact("15.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                Answer = "нет",
                CreatedAt = DateTime.ParseExact("17.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                UpdatedAt = DateTime.ParseExact("17.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };
            return new List<MedicalCertificate> { certificate1, certificate2, certificate3, certificate4 };
        }

        [Fact]
        public async void MedicalCertificateRepository_GetAllSortedAndIncludedAsync_ReturnsListOfMedicalCertificates()
        {
            //Arrange
            var certificates = GetListOfMedicalCertificates();
            var dbContext = await GetDbContext();
            var certificateRepository = new MedicalCertificateRepository(dbContext);
            foreach (var certificate in certificates)
            {
                certificateRepository.Add(certificate);
            }

            //Act
            var result = await certificateRepository.GetAllSortedAndIncludedAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(4);
            result.Should().BeOfType<List<MedicalCertificate>>();

            result[0].CertificateID.Should().Be(4);
            result[1].CertificateID.Should().Be(3);
            result[2].CertificateID.Should().Be(2);
            result[3].CertificateID.Should().Be(1);

            result[0].Student.Should().NotBeNull();
            result[0].Student.Group.Should().NotBeNull();
            result[0].Clinic.Should().NotBeNull();
            result[0].Diagnosis.Should().NotBeNull();
            result[0].User.Should().NotBeNull();
        }

        [Fact]
        public async void MedicalCertificateRepository_GetAllByTimePeriodAsync_ReturnsListOfMedicalCertificates()
        {
            //Arrange
            var certificates = GetListOfMedicalCertificates();
            var dbContext = await GetDbContext();
            var certificateRepository = new MedicalCertificateRepository(dbContext);
            foreach (var certificate in certificates)
            {
                certificateRepository.Add(certificate);
            }

            //Act
            var startDate = DateTime.ParseExact("01.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture);
            var endDate = DateTime.ParseExact("01.01.2025", "dd.MM.yyyy", CultureInfo.InvariantCulture);
            var result = await certificateRepository.GetAllByTimePeriodAsync(startDate, endDate);


            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeOfType<List<MedicalCertificate>>();
        }

        [Fact]
        public async void MedicalCertificateRepository_GetIncludedByIdAsync_ReturnsMedicalCertificate()
        {
            //Arrange
            var certificates = GetListOfMedicalCertificates();
            var dbContext = await GetDbContext();
            var certificateRepository = new MedicalCertificateRepository(dbContext);
            foreach (var certificate in certificates)
            {
                certificateRepository.Add(certificate);
            }

            //Act
            var result = await certificateRepository.GetIncludedByIdAsync(certificates[3].CertificateID);

            //Assert
            result.Should().BeOfType<MedicalCertificate>();
            result.CertificateID.Should().Be(certificates[3].CertificateID);

            result.Student.Should().NotBeNull();
            result.Student.Group.Should().NotBeNull();
            result.Clinic.Should().NotBeNull();
            result.Diagnosis.Should().NotBeNull();
            result.User.Should().NotBeNull();
        }

        [Fact]
        public async void MedicalCertificateRepository_GetAllSortedAndIncludedByStudentIdAsync_ReturnsListOfMedicalCertificates()
        {
            //Arrange
            var certificates = GetListOfMedicalCertificates();
            var dbContext = await GetDbContext();
            var certificateRepository = new MedicalCertificateRepository(dbContext);
            foreach (var certificate in certificates)
            {
                certificateRepository.Add(certificate);
            }

            //Act
            var result = await certificateRepository.GetAllSortedAndIncludedByStudentIdAsync(certificates[0].StudentID);


            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeOfType<List<MedicalCertificate>>();

            result[0].CertificateID.Should().Be(4);
            result[1].CertificateID.Should().Be(1);

            result[0].Student.Should().NotBeNull();
            result[0].Student.Group.Should().NotBeNull();
            result[0].Clinic.Should().NotBeNull();
            result[0].Diagnosis.Should().NotBeNull();
            result[0].User.Should().NotBeNull();
        }

        [Fact]
        public async void MedicalCertificateRepository_CountAsync_ReturnsInt()
        {
            //Arrange
            var certificates = GetListOfMedicalCertificates();
            var dbContext = await GetDbContext();
            var certificateRepository = new MedicalCertificateRepository(dbContext);
            foreach (var certificate in certificates)
            {
                certificateRepository.Add(certificate);
            }

            //Act
            var result = await certificateRepository.CountAsync();


            //Assert
            result.Should().Be(certificates.Count);
        }

        [Fact]
        public async void MedicalCertificateRepository_GetPagedCertificatesAsync_ReturnsListOfMedicalCertificates()
        {
            //Arrange
            var certificates = GetListOfMedicalCertificates();
            var dbContext = await GetDbContext();
            var certificateRepository = new MedicalCertificateRepository(dbContext);
            foreach (var certificate in certificates)
            {
                certificateRepository.Add(certificate);
            }

            //Act
            var result = await certificateRepository.GetPagedCertificatesAsync(2, 2);

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeOfType<List<MedicalCertificate>>();
        }

        [Fact]
        public async void MedicalCertificateRepository_GetAllIncludedAsync_ReturnsListOfMedicalCertificates()
        {
            //Arrange
            var certificates = GetListOfMedicalCertificates();
            var dbContext = await GetDbContext();
            var certificateRepository = new MedicalCertificateRepository(dbContext);
            foreach (var certificate in certificates)
            {
                certificateRepository.Add(certificate);
            }

            //Act
            var result = await certificateRepository.GetAllIncludedAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(4);
            result.Should().BeOfType<List<MedicalCertificate>>();

            result[0].Student.Should().NotBeNull();
            result[0].Student.Group.Should().NotBeNull();
            result[0].Clinic.Should().NotBeNull();
            result[0].Diagnosis.Should().NotBeNull();
            result[0].User.Should().NotBeNull();
        }

        [Fact]
        public async void MedicalCertificateRepository_GetAllByProgramIdAsync_ReturnsListOfMedicalCertificates()
        {
            //Arrange
            var certificates = GetListOfMedicalCertificates();
            var dbContext = await GetDbContext();
            var certificateRepository = new MedicalCertificateRepository(dbContext);
            foreach (var certificate in certificates)
            {
                certificateRepository.Add(certificate);
            }

            //Act
            var result = await certificateRepository.GetAllByProgramIdAsync((int)certificates[0].Student.Group.ProgramID);

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeOfType<List<MedicalCertificate>>();

            result[0].Student.Should().NotBeNull();
            result[0].Student.Group.Should().NotBeNull();
            result[0].Clinic.Should().NotBeNull();
            result[0].Diagnosis.Should().NotBeNull();
            result[0].User.Should().NotBeNull();
        }
    }
}
