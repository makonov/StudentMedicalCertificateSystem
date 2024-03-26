using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Controllers
{
    [Authorize(Policy = "UserOrAdminPolicy")]
    public class ClinicController : Controller
    {
        private readonly IClinicRepository _clinicRepository;

        public ClinicController(IClinicRepository clinicRepository)
        {
            _clinicRepository = clinicRepository;
        }

        public async Task<IActionResult> Index()
        {
            var clinics = await _clinicRepository.GetAllSortedAsync();
            return View(clinics);
        }

        public IActionResult Create()
        {
            var clinic = new Clinic();
            return View(clinic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Clinic clinic)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                return View(clinic);
            }

            _clinicRepository.Add(clinic);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var clinic = await _clinicRepository.GetByIdAsync(id);

            if (clinic == null) 
            {
                return NotFound();
            }

            return View(clinic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Clinic clinic)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                return View(clinic);
            }

            var updatedClinic = new Clinic
            {
                ClinicID = id,
                ClinicName = clinic.ClinicName
            };

            _clinicRepository.Update(updatedClinic);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var clinic = await _clinicRepository.GetByIdAsync(id);

            if (clinic == null)
            {
                return NotFound();
            }

            return View(clinic);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var clinic = await _clinicRepository.GetByIdAsync(id);

                if (clinic == null)
                {
                    return NotFound();
                }

                _clinicRepository.Delete(clinic);
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Данную больницу нельзя удалить, так как к ней прикреплены справки студентов";
            }

            return RedirectToAction("Index");
        }

    }
}
