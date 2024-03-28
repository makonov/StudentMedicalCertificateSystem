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
    public class DiagnosisRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Diagnoses.CountAsync() < 0)
            {
                databaseContext.Diagnoses.Add(new Diagnosis()
                {
                    DiagnosisName = "Диагноз 1"
                });
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void DiagnosisRepository_Add_ReturnsBool()
        {
            //Arrange
            var diagnosis = new Diagnosis()
            {
                DiagnosisName = "Диагноз 1"
            };
            var dbContext = await GetDbContext();
            var diagnosisRepository = new DiagnosisRepository(dbContext);

            //Act
            var result = diagnosisRepository.Add(diagnosis);

            //Assert
            result.Should().BeTrue();
            var addedDiagnosis = await dbContext.Diagnoses.FindAsync(diagnosis.DiagnosisID);
            addedDiagnosis.Should().NotBeNull();
            addedDiagnosis.Should().BeOfType<Diagnosis>();
        }

        [Fact]
        public async void DiagnosisRepository_Delete_ReturnsBool()
        {
            //Arrange
            var diagnosis = new Diagnosis()
            {
                DiagnosisName = "Диагноз 1"
            };
            var dbContext = await GetDbContext();
            var diagnosisRepository = new DiagnosisRepository(dbContext);

            //Act
            diagnosisRepository.Add(diagnosis);
            var result = diagnosisRepository.Delete(diagnosis);

            //Assert
            result.Should().BeTrue();
            var deletedDiagnosis = await dbContext.Diagnoses.FindAsync(diagnosis.DiagnosisID);
            deletedDiagnosis.Should().BeNull();
        }

        [Fact]
        public async void DiagnosisRepository_Update_ReturnsBool()
        {
            //Arrange
            var diagnosis = new Diagnosis()
            {
                DiagnosisName = "Диагноз 1"
            };
            var dbContext = await GetDbContext();
            var diagnosisRepository = new DiagnosisRepository(dbContext);

            //Act
            diagnosisRepository.Add(diagnosis);
            diagnosis.DiagnosisName = "Диагноз 21";
            var result = diagnosisRepository.Update(diagnosis);

            //Assert
            result.Should().BeTrue();
            var updatedDiagnosis = await dbContext.Diagnoses.FindAsync(diagnosis.DiagnosisID);
            updatedDiagnosis.DiagnosisName.Should().Be("Диагноз 21");
        }

        private List<Diagnosis> GetListOfDiagnoses()
        {
            var diagnosis1 = new Diagnosis()
            {
                DiagnosisName = "Диагноз 1"
            };
            var diagnosis2 = new Diagnosis()
            {
                DiagnosisName = "Диагноз 2"
            };
            var diagnosis3 = new Diagnosis()
            {
                DiagnosisName = "Диагноз 3"
            };
            var diagnosis4 = new Diagnosis()
            {
                DiagnosisName = "Диагноз 4"
            };
            return new List<Diagnosis>() { diagnosis1, diagnosis2, diagnosis3, diagnosis4 };
        }

        [Fact]
        public async void DiagnosisRepository_GetAllSortedAsync_ReturnsListOfDiagnoses()
        {
            //Arrange
            var diagnoses = GetListOfDiagnoses();
            var dbContext = await GetDbContext();
            var diagnosisRepository = new DiagnosisRepository(dbContext);
            foreach (var diagnosis in diagnoses)
            {
                diagnosisRepository.Add(diagnosis);
            }

            //Act
            var result = await diagnosisRepository.GetAllSortedAsync();

            //Assert
            result.Should().BeOfType<List<Diagnosis>>();
            result.Should().HaveCount(4);
            bool isSorted = true;
            for (int i = 1; i < result.Count; i++)
            {
                if (result[i].DiagnosisName.CompareTo(result[i - 1].DiagnosisName) < 0) isSorted = false;
            }
            isSorted.Should().BeTrue();
        }

        [Fact]
        public async void DiagnosisRepository_GetByIdAsync_ReturnsDiagnosis()
        {
            //Arrange
            var diagnoses = GetListOfDiagnoses();
            var dbContext = await GetDbContext();
            var diagnosisRepository = new DiagnosisRepository(dbContext);
            foreach (var diagnosis in diagnoses)
            {
                diagnosisRepository.Add(diagnosis);
            }

            //Act
            var result = await diagnosisRepository.GetByIdAsync(diagnoses[1].DiagnosisID);

            //Assert
            result.Should().BeOfType<Diagnosis>();
            result.DiagnosisID.Should().Be(diagnoses[1].DiagnosisID);
        }

        [Fact]
        public async void DiagnosisRepository_GetClinicsAsSelectListAsync_ReturnsListOfSelectedListItems()
        {
            //Arrange
            var diagnoses = GetListOfDiagnoses();
            var dbContext = await GetDbContext();
            var diagnosisRepository = new DiagnosisRepository(dbContext);
            foreach (var diagnosis in diagnoses)
            {
                diagnosisRepository.Add(diagnosis);
            }

            //Act
            var result = await diagnosisRepository.GetDiagnosesAsSelectListAsync();

            //Assert
            result.Should().BeOfType<List<SelectListItem>>();
            result.Should().HaveCount(4);
            result[0].Value.Should().NotBeNull();
            result[0].Text.Should().NotBeNull();
        }
    }
}
