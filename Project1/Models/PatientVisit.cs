using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class PatientVisit
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public DateTime VisitDate { get; set; }
        public string VisitType { get; set; }
        public string Description { get; set; }
        public string DoctorName { get; set; }
        public int DurationInMinutes { get; set; }
        public decimal Fee { get; set; }
    }
}
