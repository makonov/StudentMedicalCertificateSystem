using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using StudentMedicalCertificateSystem.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.Tests.Repository
{
    public class PhotoServiceTests
    {
        private readonly IWebHostEnvironment _webHostEnvironmentMock;
        private readonly PhotoService _photoService;

        public PhotoServiceTests()
        {
            _webHostEnvironmentMock = A.Fake<IWebHostEnvironment>();
            _photoService = new PhotoService(_webHostEnvironmentMock);
        }

        [Fact]
        public void PhotoService_IsFileUploadedAndExtensionAllowed_ValidExtension_ReturnsTrue()
        {
            // Arrange
            var file = A.Fake<IFormFile>();
            A.CallTo(() => file.FileName).Returns("test.jpg");
            A.CallTo(() => file.Length).Returns(100);

            // Act
            var result = _photoService.IsFileUploadedAndExtensionAllowed(file, new[] { ".jpg" });

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void PhotoService_IsFileUploadedAndExtensionAllowed_InvalidExtension_ReturnsFalse()
        {
            // Arrange
            var file = A.Fake<IFormFile>();
            A.CallTo(() => file.FileName).Returns("test.txt");
            A.CallTo(() => file.Length).Returns(100);

            // Act
            var result = _photoService.IsFileUploadedAndExtensionAllowed(file, new[] { ".jpg" });

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task PhotoService_UploadPhotoAsync_ValidFile_UploadsFileSuccessfully()
        {
            // Arrange
            var file = A.Fake<IFormFile>();
            A.CallTo(() => file.FileName).Returns("test.jpg");
            A.CallTo(() => file.Length).Returns(100);

            var targetFolder = "targetFolder";

            // Act
            var result = await _photoService.UploadPhotoAsync(file, targetFolder);

            // Assert
            result.IsUploadedAndExtensionValid.Should().BeTrue();
            result.FileName.Should().NotBeNull();
        }

        [Fact]
        public async Task PhotoService_ReplacePhotoAsync_ValidFile_ReplacesFileSuccessfully()
        {
            // Arrange
            var file = A.Fake<IFormFile>();
            A.CallTo(() => file.FileName).Returns("test.jpg");
            A.CallTo(() => file.Length).Returns(100);

            var targetFolder = "targetFolder";
            var existingFilePath = Path.Combine(targetFolder, "existing.jpg");
            File.Create(existingFilePath).Close();

            // Act
            var result = await _photoService.ReplacePhotoAsync(file, targetFolder, existingFilePath);

            // Assert
            result.IsReplacementSuccess.Should().BeTrue();
            result.NewFileName.Should().NotBeNull();

            // Cleanup
            File.Delete(Path.Combine(targetFolder, result.NewFileName));
        }

        [Fact]
        public async Task PhotoService_DeletePhotoAsync_ExistingFile_DeletesFileSuccessfully()
        {
            // Arrange
            var targetFolder = "targetFolder";
            var filePath = Path.Combine(targetFolder, "toDelete.jpg");
            File.Create(filePath).Close();

            // Act
            var result = await _photoService.DeletePhotoAsync(filePath);

            // Assert
            result.Should().BeTrue();
        }
    }
}

