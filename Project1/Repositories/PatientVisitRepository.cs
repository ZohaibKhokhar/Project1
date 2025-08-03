using Project1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Project1.Interfaces;

namespace Project1.Repositories
{
    public class PatientVisitRepository:IPatientVisitRepository
    {
        static  string  filePath = "visits.txt";
        static List<PatientVisit> visits = new List<PatientVisit>();
        public PatientVisitRepository() 
        {
            LoadVisits();
        }
        public void AddVisit(PatientVisit visit) 
        {
            if (visit == null)
            {
                Console.WriteLine("Visit cannot be null.");
                return;
            }
            visit.Id = visits.Count > 0 ? visits.Max(v => v.Id) + 1 : 1;
            visits.Add(visit);
            SaveVisits();
        }
        public void UpdateVisit(PatientVisit visit) 
        {
            if (visit == null || visit.Id <= 0)
            {
                Console.WriteLine("Invalid visit to update.");
                return;
            }
            var existingVisit = GetVisitById(visit.Id);
            if (existingVisit == null)
            {
                Console.WriteLine("Visit not found.");
                return;
            }
            existingVisit.PatientName = visit.PatientName;
            existingVisit.VisitDate = visit.VisitDate;
            existingVisit.VisitType = visit.VisitType;
            existingVisit.DoctorName = visit.DoctorName;
            existingVisit.Description = visit.Description;
            existingVisit.DurationInMinutes = visit.DurationInMinutes;
            SaveVisits();
        }
        public void DeleteVisit(int visitId) 
        {
            var visit = GetVisitById(visitId);
            if (visit == null)
            {
                Console.WriteLine("Visit not found.");
                return;
            }
            visits.Remove(visit);
            SaveVisits();
        }
        public PatientVisit GetVisitById(int visitId) 
        {
            var visit = visits.Find(v=>v.Id==visitId);
            if( visit == null)
            {
                Console.WriteLine("Visit not found.");
                return null;
            }
            return visit;
        }
        public List<PatientVisit> GetAllVisits() 
        {
            if (visits.Count == 0)
            {
                Console.WriteLine("No visits found.");
                return new List<PatientVisit>();
            }
            return visits;
        }

        
        public  bool IsVisitExistsInSameSlot(PatientVisit visit)
        {
            foreach (var existingVisit in visits)
            {
                if (existingVisit.PatientName == visit.PatientName &&
                    Math.Abs((existingVisit.VisitDate - visit.VisitDate).TotalMinutes) <= 30)
                {
                    return true;
                }
            }
            return false;
        }

        public void LoadVisits()
        {
            if (!File.Exists(filePath)) return;

            string[] lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var visit = JsonSerializer.Deserialize<PatientVisit>(line);
                visits.Add(visit);
            }
        }

        public void SaveAllVisits(List<PatientVisit> visits)
        {
            foreach (var visit in visits)
            {
                AddVisit(visit);
            }
            SaveVisits();
        }

        public void SaveVisits()
        {
            try
            {
                using var writer = new StreamWriter(filePath);
                foreach (var visit in visits)
                {
                    writer.WriteLine(JsonSerializer.Serialize(visit));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error saving data: " + ex.Message);
            }
        }
    }
}
