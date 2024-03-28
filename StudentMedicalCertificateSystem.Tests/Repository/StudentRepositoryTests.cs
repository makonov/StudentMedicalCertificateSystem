using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
    public class StudentRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Students.CountAsync() < 0)
            {
                databaseContext.Students.Add(
                    new Student()
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
                    });
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void StudentRepository_Add_ReturnsBool()
        {
            //Arrange
            var student = new Student()
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
            };
            var dbContext = await GetDbContext();
            var studentRepository = new StudentRepository(dbContext);

            //Act
            var result = studentRepository.Add(student);

            //Assert
            result.Should().BeTrue();
            var addedStudent = await dbContext.Students.FindAsync(student.StudentID);
            addedStudent.Should().NotBeNull();
            addedStudent.Should().BeOfType<Student>();
        }

        [Fact]
        public async void StudentRepository_Delete_ReturnsBool()
        {
            //Arrange
            var student = new Student()
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
            };
            var dbContext = await GetDbContext();
            var studentRepository = new StudentRepository(dbContext);

            //Act
            studentRepository.Add(student);
            var result = studentRepository.Delete(student);

            //Assert
            result.Should().BeTrue();
            var deletedStudent = await dbContext.Students.FindAsync(student.StudentID);
            deletedStudent.Should().BeNull();
        }

        [Fact]
        public async void StudentRepository_Update_ReturnsBool()
        {
            //Arrange
            var student = new Student()
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
            };
            var dbContext = await GetDbContext();
            var studentRepository = new StudentRepository(dbContext);

            //Act
            studentRepository.Add(student);
            student.FirstName = "Анна";
            var result = studentRepository.Update(student);


            //Assert
            result.Should().BeTrue();
            var updatedStudent = await dbContext.Students.FindAsync(student.StudentID);
            updatedStudent.FirstName.Should().Be("Анна");
        }

        private List<Student> GetListOfStudents()
        {
            var student1 = new Student()
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
            };
            var student2 = new Student()
            {
                Group = new StudentGroup()
                {
                    Program = new EducationalProgram()
                    {
                        ProgramName = "Программная инженерия"
                    },
                    GroupName = "ПИ-21-1"
                },
                LastName = "Иванов",
                FirstName = "Иван",
                Patronymic = "Иванович",
                BirthDate = DateTime.ParseExact("18.03.2002", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };
            var student3 = new Student()
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
                BirthDate = DateTime.ParseExact("19.10.2002", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };
            var student4 = new Student()
            {
                Group = new StudentGroup()
                {
                    Program = new EducationalProgram()
                    {
                        ProgramName = "Программная инженерия"
                    },
                    GroupName = "БИ-21-2"
                },
                LastName = "Максимова",
                FirstName = "Ксения",
                Patronymic = "Егоровна",
                BirthDate = DateTime.ParseExact("27.09.2002", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };
            return new List<Student> { student1, student2, student3, student4 };
        }

        [Fact]
        public async void StudentRepository_GetFullNamesWithGroupsAsSelectedListAsync_ReturnsListOfSelectedItems()
        {
            //Arrange
            var students = GetListOfStudents();
            var dbContext = await GetDbContext();
            var studentRepository = new StudentRepository(dbContext);
            foreach( var student in students )
            {
                studentRepository.Add(student);
            }

            //Act
            var result = await studentRepository.GetFullNamesWithGroupsAsSelectedListAsync();

            //Assert
            result.Should().BeOfType<List<SelectListItem>>();
            result.Should().HaveCount(4);
            result[0].Value.Should().NotBeNull();
            result[0].Text.Should().NotBeNull();
        }

        [Fact]
        public async void StudentRepository_GetIncludedByIdAsync_ReturnsStudent()
        {
            //Arrange
            var students = GetListOfStudents();
            var dbContext = await GetDbContext();
            var studentRepository = new StudentRepository(dbContext);
            foreach (var student in students)
            {
                studentRepository.Add(student);
            }

            //Act
            var result = await studentRepository.GetIncludedByIdAsync(students[3].StudentID);

            //Assert
            result.Should().BeOfType<Student>();
            result.StudentID.Should().Be(students[3].StudentID);
            result.Group.Should().NotBeNull();
            result.Group.Program.Should().NotBeNull();
        }

        [Fact]
        public async void StudentRepository_GetByFullNameAndGroupAsync_ReturnsStudent()
        {
            //Arrange
            var students = GetListOfStudents();
            var dbContext = await GetDbContext();
            var studentRepository = new StudentRepository(dbContext);
            foreach (var student in students)
            {
                studentRepository.Add(student);
            }

            //Act
            var result = await studentRepository.GetByFullNameAndGroupAsync(students[1].LastName, students[1].FirstName, students[1].Patronymic, students[1].Group.GroupName);

            //Assert
            result.Should().BeOfType<Student>();
            result.FirstName.Should().Be(students[1].FirstName);
            result.LastName.Should().Be(students[1].LastName);
            result.Patronymic.Should().Be(students[1]. Patronymic);
            result.Group.GroupName.Should().Be(students[1].Group.GroupName);
        }

        [Fact]
        public async void StudentRepository_Count_ReturnsInt()
        {
            //Arrange
            var students = GetListOfStudents();
            var dbContext = await GetDbContext();
            var studentRepository = new StudentRepository(dbContext);
            foreach (var student in students)
            {
                studentRepository.Add(student);
            }

            //Act
            var result = await studentRepository.Count();

            //Assert
            result.Should().Be(students.Count);
        }

        [Fact]
        public async void StudentRepository_GetPagedStudentsAsync_ReturnsListOfStudents()
        {
            //Arrange
            var students = GetListOfStudents();
            var dbContext = await GetDbContext();
            var studentRepository = new StudentRepository(dbContext);
            foreach (var student in students)
            {
                studentRepository.Add(student);
            }

            //Act
            var result = await studentRepository.GetPagedStudentsAsync(2, 2);

            //Assert
            result.Should().BeOfType<List<Student>>();
            result.Should().HaveCount(2); 
        }
    }
}
