﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.Repository;

namespace StudentMedicalCertificateSystem.Controllers
{
    [Authorize(Policy = "UserOrAdminPolicy")]
    public class EducationalProgramController : Controller
    {
        private readonly IEducationalProgramRepository _programRepository;

        public EducationalProgramController(IEducationalProgramRepository programRepository)
        {
            _programRepository = programRepository;
        }

        public async Task<IActionResult> Index()
        {
            var programs = await _programRepository.GetAllSortedAsync();
            return View(programs);
        }

        public IActionResult Create()
        {
            var program = new EducationalProgram();
            return View(program);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EducationalProgram program)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                return View(program);
            }

            if (_programRepository.ProgramExists(program.ProgramName))
            {
                ModelState.AddModelError("ProgramName", "Такая программа уже существует.");
                return View(program);
            }

            _programRepository.Add(program);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var program = await _programRepository.GetByIdAsync(id);

            if (program == null)
            {
                return NotFound();
            }

            return View(program);
        }

        [HttpPost]
        public IActionResult Edit(int id, string currentName, EducationalProgram program)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                return View(program);
            }

            if (currentName != program.ProgramName && _programRepository.ProgramExists(program.ProgramName))
            {
                ModelState.AddModelError("ProgramName", "Такая программа уже существует.");
                program.ProgramName = currentName;
                return View(program);
            }

            var updatedProgram = new EducationalProgram
            {
                ProgramID = id,
                ProgramName = program.ProgramName
            };

            _programRepository.Update(updatedProgram);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var program = await _programRepository.GetByIdAsync(id);

            if (program == null)
            {
                return NotFound();
            }

            return View(program);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var program = await _programRepository.GetByIdAsync(id);

                if (program == null)
                {
                    return NotFound();
                }

                _programRepository.Delete(program);
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Данную образовательную программу нельзя удалить, так как к ней привязаны группы";
            }

            return RedirectToAction("Index");
        }
    }
}
