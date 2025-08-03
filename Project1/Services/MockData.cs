using Project1.Helper;
using Project1.Interfaces;
using Project1.Models;
using Project1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Services
{
    public class MockData:IMockData
    {
        public List<PatientVisit> GenerateMockData()
        {
            List<PatientVisit> visits = new List<PatientVisit>();
            string[] patientNames = { "Ali", "Zubair", "Zohaib", "Zeeshan", "Bilal", "Khalid" };
            string[] types = { "Consultation,Follow-Up", "Emergency" };
            int[] fee = { 500, 300, 1000 };
            int[] duration = { 30, 40, 50, 60 };
            string[] doctorNames = { "Ali", "Zubair", "Zohaib", "Zeeshan", "Bilal", "Khalid" };
            Random rand = new Random();
            for (int i = 0; i < 300; i++)
            {
                visits.Add(new PatientVisit()
                {
                    PatientName = patientNames[rand.Next(patientNames.Length)],
                    VisitDate = DateTime.Now,
                    Id = visits.Count > 0 ? visits.Max(v => v.Id) + 1 : 1,
                    VisitType = types[rand.Next(types.Length)],
                    DoctorName = doctorNames[rand.Next(doctorNames.Length)],
                    Description = "this is a patient",
                    Fee = fee[rand.Next(fee.Length)],
                    DurationInMinutes = duration[rand.Next(duration.Length)]
                }); ;
            }
          return visits;
        }
    }
}
