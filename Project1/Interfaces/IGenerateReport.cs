using Project1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Interfaces
{
    public interface IGenerateReport
    {
        public void DisplayVisitReport(List<PatientVisit> visits);
    }
}
