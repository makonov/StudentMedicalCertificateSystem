using DocumentFormat.OpenXml.Spreadsheet;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StudentMedicalCertificateSystem.Controllers;
using StudentMedicalCertificateSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.Tests.Controllers
{
    public class ReferenceControllerTests
    {
        private ReferenceController _referenceController;

        public ReferenceControllerTests()
        {
            _referenceController = new ReferenceController();
        }

        [Fact]
        public void ReferenceController_Index_ReturnsSuccess()
        {
            //Arrange

            //Act
            var result = _referenceController.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
    }
}
