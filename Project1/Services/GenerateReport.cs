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
            Console.WriteLine("Visit Count by Type (Last 7 Days):");

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

            foreach (var d in dic)
                Console.WriteLine($"{d.Key}: {d.Value}");
        }
    }
}
