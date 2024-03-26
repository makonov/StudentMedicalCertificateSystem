using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.Repository;
using StudentMedicalCertificateSystem.ViewModels;

namespace StudentMedicalCertificateSystem.Controllers
{
    [Authorize(Policy = "UserOrAdminPolicy")]
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentGroupRepository _groupRepository;
        private readonly IEducationalProgramRepository _programRepository;
        private const int PageSize = 10;

        public StudentController(IStudentRepository studentRepository,
            IStudentGroupRepository groupRepository,
            IEducationalProgramRepository programRepository)
        {
            _studentRepository = studentRepository;
            _groupRepository = groupRepository;
            _programRepository = programRepository;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.StudentList = new SelectList(await GetStudents(), "Value", "Text");
     

            var totalCount = await _studentRepository.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            var students = await _studentRepository.GetPagedStudentsAsync(page, PageSize);

            var viewModel = new ShowStudentsViewModel
            {
                Students = students,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = totalCount,
                    TotalPages = totalPages
                }
            };
            return View(viewModel);
        }

        private async Task<List<SelectListItem>> GetGroups()
        {
            return await _groupRepository.GetGroupsAsSelectListAsync();
        }

        private async Task<List<SelectListItem>> GetPrograms()
        {
            return await _programRepository.GetProgramsAsSelectListAsync();
        }

        private async Task<List<SelectListItem>> GetStudents()
        {
            return await _studentRepository.GetFullNamesWithGroupsAsSelectedListAsync();
        }

        private async Task MakeLists()
        {
            ViewBag.GroupList = new SelectList(await GetGroups(), "Value", "Text");
        }

        public async Task<IActionResult> Create(bool isFromCertificate = false)
        {
            await MakeLists();
            var viewModel = new CreateStudentViewModel
            {
                IsFromCertificate = isFromCertificate
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStudentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                await MakeLists();
                return View(viewModel);
            }

            var student = new Student
            {
                GroupID = (int)viewModel.GroupID,
                LastName = viewModel.LastName,
                FirstName = viewModel.FirstName,
                Patronymic = viewModel.Patronymic,
                Course = viewModel.Course,
                BirthDate = viewModel.BirthDate.HasValue ? viewModel.BirthDate.Value : default(DateTime)
            };

            _studentRepository.Add(student);

            if (!viewModel.IsFromCertificate) 
                return RedirectToAction("Index");
            else
                return RedirectToAction("Create","MedicalCertificate");
        }

        public async Task<IActionResult> Edit(int id)
        {
            await MakeLists();
            var student = await _studentRepository.GetIncludedByIdAsync(id);
            
            if (student == null)
            {
                return NotFound();
            }

            var viewModel = new EditStudentViewModel
            {
                StudentID = student.StudentID,
                GroupID = student.GroupID,
                LastName = student.LastName,
                FirstName = student.FirstName,
                Patronymic = student.Patronymic,
                Course = student.Course,
                BirthDate = student.BirthDate
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditStudentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                await MakeLists();
                return View(viewModel);
            }

            var student = new Student
            {
                StudentID = id,
                GroupID = (int)viewModel.GroupID,
                LastName = viewModel.LastName,
                FirstName = viewModel.FirstName,
                Patronymic = viewModel.Patronymic,
                Course = viewModel.Course,
                BirthDate = viewModel.BirthDate.HasValue ? viewModel.BirthDate.Value : default(DateTime)
            };

            _studentRepository.Update(student);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentRepository.GetIncludedByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _studentRepository.GetIncludedByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            _studentRepository.Delete(student);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Find(FindStudentViewModel viewModel)
        {
            ViewBag.StudentList = new SelectList(await GetStudents(), "Value", "Text");
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                return View(viewModel);
            }

            string[] studentData = viewModel.StudentData.Split(" - ");
            string[] fullName = studentData[0].Split();
            string groupName = studentData[1];
            string lastName = fullName[0];
            string firstName = fullName[1];
            string patronymic = fullName[2];
            Student foundStudent = await _studentRepository.GetByFullNameAndGroupAsync(lastName, firstName, patronymic, groupName);
            ShowStudentsViewModel newViewModel = new ShowStudentsViewModel
            {
                Students = new List<Student> { foundStudent },
                PagingInfo = new PagingInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = PageSize,
                    TotalItems = 1,
                    TotalPages = 1
                }
            };
            return View("Index", newViewModel);
        }
    }
}
