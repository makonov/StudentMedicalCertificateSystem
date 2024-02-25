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

            var students = await _studentRepository.GetPagedStudents(page, PageSize);

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
            return await _groupRepository.GetGroupsAsSelectList();
        }

        private async Task<List<SelectListItem>> GetPrograms()
        {
            return await _programRepository.GetProgramsAsSelectList();
        }

        private async Task<List<SelectListItem>> GetStudents()
        {
            return await _studentRepository.GetStudentFullNamesWithGroupsAsSelectedList();
        }

        public async Task MakeLists()
        {
            ViewBag.GroupList = new SelectList(await GetGroups(), "Value", "Text");
        }

        public async Task<IActionResult> Create()
        {
            await MakeLists();
            var viewModel = new CreateStudentViewModel();
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

            return RedirectToAction("Index");   
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
        public async Task<IActionResult> Find(FindStudentViewModel findViewModel)
        {
            ViewBag.StudentList = new SelectList(await GetStudents(), "Value", "Text");
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Произошла ошибка");
                return View(findViewModel);
            }

            string[] studentData = findViewModel.StudentData.Split(" - ");
            string[] fullName = studentData[0].Split();
            string groupName = studentData[1];
            string lastName = fullName[0];
            string firstName = fullName[1];
            string patronymic = fullName[2];
            Student foundStudent = await _studentRepository.GetByFullNameAndGroup(lastName, firstName, patronymic, groupName);
            ShowStudentsViewModel viewModel = new ShowStudentsViewModel
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
            return View("Index", viewModel);
        }
    }
}
