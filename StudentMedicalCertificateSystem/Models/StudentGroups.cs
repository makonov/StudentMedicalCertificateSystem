using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks.Dataflow;

namespace StudentMedicalCertificateSystem.Models
{
    public class StudentGroups
    {
        [Key]
        public int GroupID { get; set; }
        public string? GroupName { get; set; }
    }
}
