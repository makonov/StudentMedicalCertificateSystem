using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.Repository;

namespace StudentMedicalCertificateSystem.Controllers
{
    [Authorize(Policy = "UserOrAdminPolicy")]
    public class EducationalOfficeController : Controller
    {
        private readonly IEducationalOfficeRepository _officeRepository;

        public EducationalOfficeController(IEducationalOfficeRepository officeRepository)
        {
            _officeRepository = officeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var offices = await _officeRepository.GetAllSorted();
            return View(offices);
        }

        public async Task<IActionResult> Create()
        {
            var office = new EducationalOffice();
            return View(office);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EducationalOffice office)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                return View(office);
            }

            _officeRepository.Add(office);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var office = await _officeRepository.GetByIdAsync(id);

            if (office == null)
            {
                return NotFound();
            }

            return View(office);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EducationalOffice office)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                return View(office);
            }

            var updatedOffice = new EducationalOffice
            {
                OfficeID = id,
                OfficeName = office.OfficeName
            };

            _officeRepository.Update(updatedOffice);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var office = await _officeRepository.GetByIdAsync(id);

            if (office == null)
            {
                return NotFound();
            }

            return View(office);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var office = await _officeRepository.GetByIdAsync(id);

                if (office == null)
                {
                    return NotFound();
                }

                _officeRepository.Delete(office);
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Данный офис нельзя удалить, так как к нему прикреплены студенты и/или пользователи";
            }

            return RedirectToAction("Index");
        }
    }
}
