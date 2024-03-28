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
    public class EducationalProgramRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.EducationalPrograms.CountAsync() < 0)
            {
                databaseContext.EducationalPrograms.Add(new EducationalProgram()
                {
                    ProgramName = "Программная инженерия"
                });
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void EducationalProgramRepository_Add_ReturnsBool()
        {
            //Arrange
            var program = new EducationalProgram()
            {
                ProgramName = "Программная инженерия"
            };
            var dbContext = await GetDbContext();
            var programRepository = new EducationalProgramRepository(dbContext);

            //Act
            var result = programRepository.Add(program);

            //Assert
            result.Should().BeTrue();
            var addedProgram = await dbContext.EducationalPrograms.FindAsync(program.ProgramID);
            addedProgram.Should().NotBeNull();
            addedProgram.Should().BeOfType<EducationalProgram>();
        }

        [Fact]
        public async void EducationalProgramRepository_Delete_ReturnsBool()
        {
            //Arrange
            var program = new EducationalProgram()
            {
                ProgramName = "Программная инженерия"
            };
            var dbContext = await GetDbContext();
            var programRepository = new EducationalProgramRepository(dbContext);

            //Act
            programRepository.Add(program);
            var result = programRepository.Delete(program);

            //Assert
            result.Should().BeTrue();
            var deletedProgram = await dbContext.EducationalPrograms.FindAsync(program.ProgramID);
            deletedProgram.Should().BeNull();
        }

        [Fact]
        public async void EducationalProgramRepository_Update_ReturnsBool()
        {
            //Arrange
            var program = new EducationalProgram()
            {
                ProgramName = "Программная инженерия"
            };
            var dbContext = await GetDbContext();
            var programRepository = new EducationalProgramRepository(dbContext);

            //Act
            programRepository.Add(program);
            program.ProgramName = "Бизнес-информатика";
            var result = programRepository.Update(program);

            //Assert
            result.Should().BeTrue();
            var updatedProgram = await dbContext.EducationalPrograms.FindAsync(program.ProgramID);
            updatedProgram.ProgramName.Should().Be("Бизнес-информатика");
        }

        private List<EducationalProgram> GetListOfPrograms()
        {
            var program1 = new EducationalProgram()
            {
                ProgramName = "Программная инженерия"
            };
            var program2 = new EducationalProgram()
            {
                ProgramName = "Бизнес-информатика"
            };
            var program3 = new EducationalProgram()
            {
                ProgramName = "Информатика и вычислительная техника"
            };
            var program4 = new EducationalProgram()
            {
                ProgramName = "Прикладная информатика"
            };
            return new List<EducationalProgram>() { program1, program2 , program3 , program4 };
        }

        [Fact]
        public async void EducationalProgramRepository_GetProgramsAsSelectListAsync_ReturnsListOfSelectedListItems()
        {
            //Arrange
            var programs = GetListOfPrograms();
            var dbContext = await GetDbContext();
            var programRepository = new EducationalProgramRepository(dbContext);
            foreach (var program in programs)
            {
                programRepository.Add(program);
            }

            //Act
            var result = await programRepository.GetProgramsAsSelectListAsync();

            //Assert
            result.Should().BeOfType<List<SelectListItem>>();
            result.Should().HaveCount(4);
            result[0].Value.Should().NotBeNull();
            result[0].Text.Should().NotBeNull();
        }

        [Fact]
        public async void EducationalProgramRepository_GetAllSortedAsync_ReturnsListOfPrograms()
        {
            //Arrange
            var programs = GetListOfPrograms();
            var dbContext = await GetDbContext();
            var programRepository = new EducationalProgramRepository(dbContext);
            foreach (var program in programs)
            {
                programRepository.Add(program);
            }

            //Act
            var result = await programRepository.GetAllSortedAsync();

            //Assert
            result.Should().BeOfType<List<EducationalProgram>>();
            result.Should().HaveCount(4);
            bool isSorted = true;
            for (int i = 1; i < result.Count; i++)
            {
                if (result[i].ProgramName.CompareTo(result[i-1].ProgramName) < 0) isSorted = false;
            }
            isSorted.Should().BeTrue();
        }

        [Fact]
        public async void EducationalProgramRepository_GetByIdAsync_ReturnsProgram()
        {
            //Arrange
            var programs = GetListOfPrograms();
            var dbContext = await GetDbContext();
            var programRepository = new EducationalProgramRepository(dbContext);
            foreach (var program in programs)
            {
                programRepository.Add(program);
            }

            //Act
            var result = await programRepository.GetByIdAsync(programs[1].ProgramID);

            //Assert
            result.Should().BeOfType<EducationalProgram>();
            result.ProgramID.Should().Be(programs[1].ProgramID);
        }
    }
}
