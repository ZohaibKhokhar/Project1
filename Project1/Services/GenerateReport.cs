using Project1.Interfaces;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Services
{
    public class GenerateReport:IGenerateReport
    {
        public void DisplayVisitReport(List<PatientVisit> visits)
        {
            Console.WriteLine("\n========== Visit Report (Last 7 Days) ==========");

            DateTime days = DateTime.Now.AddDays(-7);
            var recentVisits = visits.Where(v => v.VisitDate >= days).ToList();

            if (recentVisits.Count == 0)
            {
                Console.WriteLine("No visits found in the last 7 days.");
                return;
            }

            Dictionary<string, int> dic = new Dictionary<string, int>();

            foreach (var v in recentVisits)
            {
                if (!dic.ContainsKey(v.VisitType))
                    dic[v.VisitType] = 0;
                dic[v.VisitType]++;
            }

            Console.WriteLine("\nVisit Count by Type:");
            foreach (var d in dic)
                Console.WriteLine($"- {d.Key}: {d.Value}");
        }

        public void DisplaySummary(PatientVisit visit)
        {
            if (visit == null)
            {
                Console.WriteLine("Visit not found.");
                return;
            }

            Console.WriteLine("\n========== Visit Summary ==========\n");

            Console.WriteLine($"Patient Name       : {visit.PatientName}");
            Console.WriteLine($"Visit Date & Time  : {visit.VisitDate}");
            Console.WriteLine($"Visit Type         : {visit.VisitType}");
            Console.WriteLine($"Description        : {visit.Description}");
            Console.WriteLine($"Doctor Name        : {visit.DoctorName}");
            Console.WriteLine($"Duration           : {visit.DurationInMinutes} minutes");
            Console.WriteLine($"Fee                : {visit.Fee}");
        }
    }
}
