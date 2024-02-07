using System.ComponentModel.DataAnnotations;

namespace StudentMedicalCertificateSystem.ViewModels
{
    public class FilterCertificatesViewModel
    {
        public string StudentData { get; set; }
        [DataType(DataType.Date)]
        public DateTime? IlnessDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? RecoveryDate { get; set; }
    }
}
