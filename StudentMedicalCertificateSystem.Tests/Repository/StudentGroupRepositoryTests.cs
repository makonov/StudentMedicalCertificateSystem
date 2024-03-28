using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.Tests.Repository
{
    public class StudentGroupRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.StudentGroups.CountAsync() < 0)
            {
                databaseContext.StudentGroups.Add(new StudentGroup()
                {
                    Program = new EducationalProgram()
                    {
                        ProgramName = "Программная инженерия"
                    },
                    GroupName = "ПИ-21-1"
                });
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void StudentGroupRepository_Add_ReturnsBool()
        {
            //Arrange
            var group = new StudentGroup()
            {
                Program = new EducationalProgram()
                {
                    ProgramName = "Программная инженерия"
                },
                GroupName = "ПИ-21-2"
            };
            var dbContext = await GetDbContext();
            var groupRepository = new StudentGroupRepository(dbContext);

            //Act
            var result = groupRepository.Add(group);

            //Assert
            result.Should().BeTrue();
            var addedGroup = await dbContext.StudentGroups.FindAsync(group.GroupID);
            addedGroup.Should().NotBeNull();
            addedGroup.Should().BeOfType<StudentGroup>();
        }

        [Fact]
        public async void StudentGroupRepository_Delete_ReturnsBool()
        {
            //Arrange
            var group = new StudentGroup()
            {
                Program = new EducationalProgram()
                {
                    ProgramName = "Программная инженерия"
                },
                GroupName = "ПИ-21-2"
            };
            var dbContext = await GetDbContext();
            var groupRepository = new StudentGroupRepository(dbContext);

            //Act
            groupRepository.Add(group);
            var result = groupRepository.Delete(group);

            //Assert
            result.Should().BeTrue();
            var deletedGroup = await dbContext.StudentGroups.FindAsync(group.GroupID);
            deletedGroup.Should().BeNull();
        }

        [Fact]
        public async void StudentGroupRepository_Update_ReturnsBool()
        {
            //Arrange
            var group = new StudentGroup()
            {
                Program = new EducationalProgram()
                {
                    ProgramName = "Программная инженерия"
                },
                GroupName = "ПИ-21-2"
            };
            var dbContext = await GetDbContext();
            var groupRepository = new StudentGroupRepository(dbContext);

            //Act
            groupRepository.Add(group);
            group.GroupName = "ПИ-22-2";
            var result = groupRepository.Update(group);

            //Assert
            result.Should().BeTrue();
            var updatedGroup = await dbContext.StudentGroups.FindAsync(group.GroupID);
            updatedGroup.GroupName.Should().Be("ПИ-22-2");
        }

        private List<StudentGroup> GetListOfGroups()
        {
            var group1 = new StudentGroup()
            {
                Program = new EducationalProgram()
                {
                    ProgramName = "Программная инженерия"
                },
                GroupName = "ПИ-21-1"
            };
            var group2 = new StudentGroup()
            {
                Program = new EducationalProgram()
                {
                    ProgramName = "Программная инженерия"
                },
                GroupName = "ПИ-21-2"
            };
            var group3 = new StudentGroup()
            {
                Program = new EducationalProgram()
                {
                    ProgramName = "Программная инженерия"
                },
                GroupName = "ПИ-21-3"
            };
            var group4 = new StudentGroup()
            {
                Program = new EducationalProgram()
                {
                    ProgramName = "Программная инженерия"
                },
                GroupName = "ПИ-21-4"
            };
            return new List<StudentGroup>() { group1, group2, group3, group4 };
        }

        [Fact]
        public async void StudentGroupRepository_GetAllSortedAsync_ReturnsListOfGroups()
        {
            //Arrange
            var groups = GetListOfGroups();
            var dbContext = await GetDbContext();
            var groupRepository = new StudentGroupRepository(dbContext);
            foreach (var group in groups)
            {
                groupRepository.Add(group);
            }

            //Act
            var result = await groupRepository.GetAllSortedAsync();

            //Assert
            result.Should().BeOfType<List<StudentGroup>>();
            result.Should().HaveCount(4);
            bool isSorted = true;
            for (int i = 1; i < result.Count; i++)
            {
                if (result[i].GroupName.CompareTo(result[i - 1].GroupName) < 0) isSorted = false;
            }
            isSorted.Should().BeTrue();
        }

        [Fact]
        public async void StudentGroupRepository_GetByIdAsync_ReturnsGroup()
        {
            //Arrange
            var groups = GetListOfGroups();
            var dbContext = await GetDbContext();
            var groupRepository = new StudentGroupRepository(dbContext);
            foreach (var group in groups)
            {
                groupRepository.Add(group);
            }

            //Act
            var result = await groupRepository.GetByIdAsync(groups[1].GroupID);

            //Assert
            result.Should().BeOfType<StudentGroup>();
            result.GroupID.Should().Be(groups[1].GroupID);
        }

        [Fact]
        public async void StudentGroupRepository_GetGroupsAsSelectListAsync_ReturnsListOfSelectedListItems()
        {
            //Arrange
            var groups = GetListOfGroups();
            var dbContext = await GetDbContext();
            var groupRepository = new StudentGroupRepository(dbContext);
            foreach (var group in groups)
            {
                groupRepository.Add(group);
            }

            //Act
            var result = await groupRepository.GetGroupsAsSelectListAsync();

            //Assert
            result.Should().BeOfType<List<SelectListItem>>();
            result.Should().HaveCount(4);
            result[0].Value.Should().NotBeNull();
            result[0].Text.Should().NotBeNull();
        }
    }
}
