using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Controllers
{
    [Authorize(Policy = "UserOrAdminPolicy")]
    public class StudentGroupController : Controller
    {
        private readonly IStudentGroupRepository _groupRepository;
        private const int PageSize = 20;

        public StudentGroupController(IStudentGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<IActionResult> Index()
        {
            var groups = await _groupRepository.GetAllSorted();
            return View(groups);
        }

        public IActionResult Create()
        {
            var group = new StudentGroup();
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentGroup group)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                return View(group);
            }

            _groupRepository.Add(group);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, StudentGroup group)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                return View(group);
            }

            var updatedGroup = new StudentGroup
            {
                GroupID = id,
                GroupName = group.GroupName
            };

            _groupRepository.Update(updatedGroup);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var group = await _groupRepository.GetByIdAsync(id);

                if (group == null)
                {
                    return NotFound();
                }

                _groupRepository.Delete(group);
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Данную группу нельзя удалить, так как к ней прикреплены студенты";
            }

            return RedirectToAction("Index");
        }
    }
}
