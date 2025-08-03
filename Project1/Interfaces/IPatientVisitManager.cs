using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Interfaces
{
    public interface IPatientVisitManager
    {
        void AddVisit();
        void UpdateVisit();
        void DeleteVisit();
        void SearchVisits();
        void ShowReports();
        void Undo();
        void Redo();
        void GenerateMockData();
        void FilterAndSortVisits();
        void ShowSummary();
    }
}
