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
    public class ClinicRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Clinics.CountAsync() < 0)
            {
                databaseContext.Clinics.Add(new Clinic()
                {
                    ClinicName = "Больница №1"
                });
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void ClinicRepository_Add_ReturnsBool()
        {
            //Arrange
            var clinic = new Clinic()
            {
                ClinicName = "Больница №1"
            };
            var dbContext = await GetDbContext();
            var clinicRepository = new ClinicRepository(dbContext);

            //Act
            var result = clinicRepository.Add(clinic);

            //Assert
            result.Should().BeTrue();
            var addedClinic = await dbContext.Clinics.FindAsync(clinic.ClinicID);
            addedClinic.Should().NotBeNull();
            addedClinic.Should().BeOfType<Clinic>();
        }

        [Fact]
        public async void ClinicRepository_Delete_ReturnsBool()
        {
            //Arrange
            var clinic = new Clinic()
            {
                ClinicName = "Больница №1"
            };
            var dbContext = await GetDbContext();
            var clinicRepository = new ClinicRepository(dbContext);

            //Act
            clinicRepository.Add(clinic);
            var result = clinicRepository.Delete(clinic);

            //Assert
            result.Should().BeTrue();
            var deletedClinic = await dbContext.Clinics.FindAsync(clinic.ClinicID);
            deletedClinic.Should().BeNull();
        }

        [Fact]
        public async void ClinicRepository_Update_ReturnsBool()
        {
            //Arrange
            var clinic = new Clinic()
            {
                ClinicName = "Больница №1"
            };
            var dbContext = await GetDbContext();
            var clinicRepository = new ClinicRepository(dbContext);

            //Act
            clinicRepository.Add(clinic);
            clinic.ClinicName = "Больница №2";
            var result = clinicRepository.Update(clinic);

            //Assert
            result.Should().BeTrue();
            var updatedClinic = await dbContext.Clinics.FindAsync(clinic.ClinicID);
            updatedClinic.ClinicName.Should().Be("Больница №2");
        }

        private List<Clinic> GetListOfClinics()
        {
            var clinic1 = new Clinic()
            {
                ClinicName = "Больница №1"
            };
            var clinic2 = new Clinic()
            {
                ClinicName = "Больница №2"
            };
            var clinic3 = new Clinic()
            {
                ClinicName = "Больница №3"
            };
            var clinic4 = new Clinic()
            {
                ClinicName = "Больница №4"
            };
            return new List<Clinic>() { clinic1, clinic2, clinic3, clinic4 };
        }

        [Fact]
        public async void ClinicRepository_GetAllSortedAsync_ReturnsListOfClinics()
        {
            //Arrange
            var clinics = GetListOfClinics();
            var dbContext = await GetDbContext();
            var clinicRepository = new ClinicRepository(dbContext);
            foreach (var clinic in clinics)
            {
                clinicRepository.Add(clinic);
            }

            //Act
            var result = await clinicRepository.GetAllSortedAsync();

            //Assert
            result.Should().BeOfType<List<Clinic>>();
            result.Should().HaveCount(4);
            bool isSorted = true;
            for (int i = 1; i < result.Count; i++)
            {
                if (result[i].ClinicName.CompareTo(result[i - 1].ClinicName) < 0) isSorted = false;
            }
            isSorted.Should().BeTrue();
        }

        [Fact]
        public async void ClinicRepository_GetByIdAsync_ReturnsClinic()
        {
            //Arrange
            var clinics = GetListOfClinics();
            var dbContext = await GetDbContext();
            var clinicRepository = new ClinicRepository(dbContext);
            foreach (var clinic in clinics)
            {
                clinicRepository.Add(clinic);
            }

            //Act
            var result = await clinicRepository.GetByIdAsync(clinics[1].ClinicID);

            //Assert
            result.Should().BeOfType<Clinic>();
            result.ClinicID.Should().Be(clinics[1].ClinicID);
        }

        [Fact]
        public async void ClinicRepository_GetClinicsAsSelectListAsync_ReturnsListOfSelectedListItems()
        {
            //Arrange
            var clinics = GetListOfClinics();
            var dbContext = await GetDbContext();
            var clinicRepository = new ClinicRepository(dbContext);
            foreach (var clinic in clinics)
            {
                clinicRepository.Add(clinic);
            }

            //Act
            var result = await clinicRepository.GetClinicsAsSelectListAsync();

            //Assert
            result.Should().BeOfType<List<SelectListItem>>();
            result.Should().HaveCount(4);
            result[0].Value.Should().NotBeNull();
            result[0].Text.Should().NotBeNull();
        }
    }
}
