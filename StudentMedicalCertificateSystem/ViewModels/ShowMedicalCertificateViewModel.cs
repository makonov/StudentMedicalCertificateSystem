using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class ShowMedicalCertificateViewModel
    {
        public List<MedicalCertificate> Certificates { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public bool IsFiltered { get; set; } = false;
        public FilterCertificatesViewModel FilterViewModel { get; set; }
    }
}
