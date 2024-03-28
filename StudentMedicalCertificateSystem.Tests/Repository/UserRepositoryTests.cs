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
    public class UserRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Users.CountAsync() < 0)
            {
                databaseContext.Users.Add(new User()
                {
                    UserName = "officeWorker",
                    PasswordHash = "HashCode",
                    LastName = "Сахипова",
                    FirstName = "Марина",
                    Patronymic = "Станиславовна"
                });
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void UserRepository_Add_ReturnsBool()
        {
            //Arrange
            var user = new User()
            {
                UserName = "officeWorker1",
                PasswordHash = "HashCode1",
                LastName = "Аркадьева",
                FirstName = "Лидия",
                Patronymic = "Ивановна"
            };
            var dbContext = await GetDbContext();
            var userRepository = new UserRepository(dbContext);

            //Act
            var result = userRepository.Add(user);

            //Assert
            result.Should().BeTrue();
            var addedUser = await dbContext.Users.FindAsync(user.Id);
            addedUser.Should().NotBeNull();
            addedUser.Should().BeOfType<User>();
        }

        [Fact]
        public async void UserRepository_Delete_ReturnsBool()
        {
            //Arrange
            var user = new User()
            {
                UserName = "officeWorker1",
                PasswordHash = "HashCode1",
                LastName = "Аркадьева",
                FirstName = "Лидия",
                Patronymic = "Ивановна"
            };
            var dbContext = await GetDbContext();
            var userRepository = new UserRepository(dbContext);

            //Act
            userRepository.Add(user);
            var result = userRepository.Delete(user);

            //Assert
            result.Should().BeTrue();
            var deletedUser = await dbContext.Users.FindAsync(user.Id);
            deletedUser.Should().BeNull();
        }

        [Fact]
        public async void UserRepository_Update_ReturnsBool()
        {
            //Arrange
            var user = new User()
            {
                UserName = "officeWorker1",
                PasswordHash = "HashCode1",
                LastName = "Аркадьева",
                FirstName = "Лидия",
                Patronymic = "Ивановна"
            };
            var dbContext = await GetDbContext();
            var userRepository = new UserRepository(dbContext);

            //Act
            userRepository.Add(user);
            user.LastName = "Прохорова";
            var result = userRepository.Update(user);

            //Assert
            result.Should().BeTrue();
            var updatedUser = await dbContext.Users.FindAsync(user.Id);
            updatedUser.LastName.Should().Be("Прохорова");
        }

        private List<User> GetListOfUsers()
        {
            var user1 = new User()
            {
                UserName = "officeWorker1",
                PasswordHash = "HashCode1",
                LastName = "Аркадьева",
                FirstName = "Лидия",
                Patronymic = "Ивановна"
            };
            var user2 = new User()
            {
                UserName = "officeWorker2",
                PasswordHash = "HashCode2",
                LastName = "Иванов",
                FirstName = "Иван",
                Patronymic = "Иванович"
            };
            var user3 = new User()
            {
                UserName = "officeWorker3",
                PasswordHash = "HashCode3",
                LastName = "Петров",
                FirstName = "Петр",
                Patronymic = "Петрович"
            };
            var user4 = new User()
            {
                UserName = "officeWorker4",
                PasswordHash = "HashCode4",
                LastName = "Данилова",
                FirstName = "Анна",
                Patronymic = "Егоровна"
            };
            return new List<User>() { user1, user2, user3, user4 };
        }

        [Fact]
        public async void UserRepository_GetAllAsync_ReturnsListOfUsers()
        {
            //Arrange
            var users = GetListOfUsers();
            var dbContext = await GetDbContext();
            var userRepository = new UserRepository(dbContext);
            foreach (var user in users)
            {
                userRepository.Add(user);
            }

            //Act
            var result = await userRepository.GetAllAsync();

            //Assert
            result.Should().BeOfType<List<User>>();
            result.Should().HaveCount(4);
        }

        [Fact]
        public async void UserRepository_GetByIdAsync_ReturnsUser()
        {
            //Arrange
            var users = GetListOfUsers();
            var dbContext = await GetDbContext();
            var userRepository = new UserRepository(dbContext);
            foreach (var user in users)
            {
                userRepository.Add(user);
            }

            //Act
            var result = await userRepository.GetByIdAsync(users[1].Id);

            //Assert
            result.Should().BeOfType<User>();
            result.Id.Should().Be(users[1].Id);
        }

        [Fact]
        public async void UserRepository_GetUsersAsSelectListAsync_ReturnsListOfSelectedListItems()
        {
            //Arrange
            var users = GetListOfUsers();
            var dbContext = await GetDbContext();
            var userRepository = new UserRepository(dbContext);
            foreach (var user in users)
            {
                userRepository.Add(user);
            }

            //Act
            var result = await userRepository.GetUsersAsSelectListAsync();

            //Assert
            result.Should().BeOfType<List<SelectListItem>>();
            result.Should().HaveCount(4);
            result[0].Value.Should().NotBeNull();
            result[0].Text.Should().NotBeNull();
        }
    }
}
