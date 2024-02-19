using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.Repository;

namespace StudentMedicalCertificateSystem.Controllers
{
    [Authorize(Policy = "UserOrAdminPolicy")]
    public class DiagnosisController : Controller
    {
        private readonly IDiagnosisRepository _diagnosisRepository;

        public DiagnosisController(IDiagnosisRepository diagnosisRepository)
        {
            _diagnosisRepository = diagnosisRepository;
        }

        public async Task<IActionResult> Index()
        {
            var diagnoses = await _diagnosisRepository.GetAllSorted();
            return View(diagnoses);
        }

        public async Task<IActionResult> Create()
        {
            var diagnosis = new Diagnosis();
            return View(diagnosis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Diagnosis diagnosis)
        {
            if (diagnosis.Code == null && diagnosis.DiagnosisName == null)
            {
                ModelState.AddModelError("", "Хотя бы одно поле должно быть заполнено");
                return View(diagnosis);
            }

            _diagnosisRepository.Add(diagnosis);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var diagnosis = await _diagnosisRepository.GetByIdAsync(id);

            if (diagnosis == null)
            {
                return NotFound();
            }

            return View(diagnosis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Diagnosis diagnosis)
        {
            if (diagnosis.Code == null && diagnosis.DiagnosisName == null)
            {
                ModelState.AddModelError("", "Хотя бы одно поле должно быть заполнено");
                return View(diagnosis);
            }

            var updatedDiagnosis = new Diagnosis
            {
                DiagnosisID = id,
                DiagnosisName = diagnosis.DiagnosisName,
                Code = diagnosis.Code
            };

            _diagnosisRepository.Update(updatedDiagnosis);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var diagnosis = await _diagnosisRepository.GetByIdAsync(id);

            if (diagnosis == null)
            {
                return NotFound();
            }

            return View(diagnosis);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var diagnosis = await _diagnosisRepository.GetByIdAsync(id);

                if (diagnosis == null)
                {
                    return NotFound();
                }

                _diagnosisRepository.Delete(diagnosis);
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Данный диагноз нельзя удалить, так как к нему прикреплены справки студентов";
            }

            return RedirectToAction("Index");
        }
    }
}
