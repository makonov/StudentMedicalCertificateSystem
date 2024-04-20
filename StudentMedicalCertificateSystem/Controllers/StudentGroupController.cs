using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Controllers
{
    [Authorize(Policy = "UserOrAdminPolicy")]
    public class StudentGroupController : Controller
    {
        private readonly IStudentGroupRepository _groupRepository;
        private readonly IEducationalProgramRepository _programRepository;

        public StudentGroupController(IStudentGroupRepository groupRepository,
            IEducationalProgramRepository programRepository)
        {
            _groupRepository = groupRepository;
            _programRepository = programRepository;
        }

        public async Task<IActionResult> Index()
        {
            var groups = await _groupRepository.GetAllSortedAsync();
            return View(groups);
        }

        private async Task<List<SelectListItem>> GetPrograms()
        {
            return await _programRepository.GetProgramsAsSelectListAsync();
        }

        public async Task<IActionResult> Create()
        {
            var group = new StudentGroup();
            ViewBag.ProgramList = new SelectList(await GetPrograms(), "Value", "Text");
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentGroup group)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                ViewBag.ProgramList = new SelectList(await GetPrograms(), "Value", "Text");
                return View(group);
            }

            if (_groupRepository.GroupExists(group.GroupName))
            {
                ModelState.AddModelError("GroupName", "Такая группа уже существует.");
                ViewBag.ProgramList = new SelectList(await GetPrograms(), "Value", "Text");
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

            ViewBag.ProgramList = new SelectList(await GetPrograms(), "Value", "Text");
            return View(group);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string currentName, StudentGroup group)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                ViewBag.ProgramList = new SelectList(await GetPrograms(), "Value", "Text");
                return View(group);
            }

            if (currentName != group.GroupName && _groupRepository.GroupExists(group.GroupName))
            {
                ModelState.AddModelError("GroupName", "Такая группа уже существует.");
                ViewBag.ProgramList = new SelectList(await GetPrograms(), "Value", "Text");
                group.GroupName = currentName;
                return View(group);
            }

            var updatedGroup = new StudentGroup
            {
                GroupID = id,
                ProgramID = group.ProgramID,
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
            var group = await _groupRepository.GetByIdAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            _groupRepository.Delete(group);

            return RedirectToAction("Index");
        }
    }
}
