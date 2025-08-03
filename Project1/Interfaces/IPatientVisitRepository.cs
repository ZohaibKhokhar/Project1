using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project1.Models;

namespace Project1.Interfaces
{
    public interface IPatientVisitRepository
    {
        public void AddVisit(PatientVisit visit);
        public void UpdateVisit(PatientVisit visit);
        public void DeleteVisit(int visitId);
        public PatientVisit GetVisitById(int visitId);
        public List<PatientVisit> GetAllVisits();

        public void SaveAllVisits(List<PatientVisit> visits);
        bool IsVisitExistsInSameSlot(PatientVisit visit);
    }
}
