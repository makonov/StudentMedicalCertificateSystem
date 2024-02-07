using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class ShowStudentsViewModel
    {
        public List<Student> Students { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
