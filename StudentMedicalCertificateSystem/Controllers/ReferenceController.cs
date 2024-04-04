using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentMedicalCertificateSystem.Interfaces;

namespace StudentMedicalCertificateSystem.Controllers
{
    [Authorize(Policy = "UserOrAdminPolicy")]
    public class ReferenceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
