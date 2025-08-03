using Project1.Interfaces;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Project1.Repositories;

namespace Project1.Services
{
    public class PatientVisitManager:IPatientVisitManager
    {
        private readonly ILogger _logger;
        private readonly IGenerateReport _reportGenerator;
        private readonly IPatientVisitRepository _patientVisitRepository;
        private readonly ILoadConsultationFee _loadFee;
        static List<PatientVisit> visits = new List<PatientVisit>();
        static Stack<string> undo = new Stack<string>();
        static Stack<string> redo = new Stack<string>();

        public PatientVisitManager(ILogger logger,IGenerateReport reportGenerator,IPatientVisitRepository patientVisitRepository,ILoadConsultationFee loadFee)
        {
            _logger=logger;
            _reportGenerator=reportGenerator;
            _patientVisitRepository=patientVisitRepository;
            _loadFee=loadFee;
        }
        public void AddVisit()
        {
            PatientVisit visit = new PatientVisit();
            Console.Write("Enter Patient Name : ");
            visit.PatientName = Console.ReadLine();
            Console.Write("Enter visit date and time (yyyy-MM-dd HH:mm): ");
            string inputDate = Console.ReadLine();

            DateTime visitDate;
            while (!DateTime.TryParseExact(inputDate, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out visitDate))
            {
                Console.Write("Invalid format. Please enter again (yyyy-MM-dd HH:mm): ");
                inputDate = Console.ReadLine();
            }
          
            
                if (_patientVisitRepository.IsVisitExistsInSameSlot(visit))
                {
                    Console.WriteLine("Warning: This patient has another visit within 30 minutes. Proceed? (Y/N)");
                    string op = Console.ReadLine();
                    if (op?.ToUpper() != "Y")
                    {
                        Console.WriteLine("Visit not added due to conflict.");
                        _logger.LogActivity("Add Visit (Conflict)", false);
                        return;
                    }
                }
          
            visit.VisitDate = visitDate;
            string  input;
            do 
            { 
                Console.Write("Enter visit type (1. Consultation, 2. Follow-Up, 3. Emergency): ");
                input = Console.ReadLine();
            } while (input!="1"&& input != "2" && input != "3" );
            if (input=="1")
                visit.VisitType = "Consultation";
            else if (input == "2")
                visit.VisitType = "Follow-Up";
            else if (input == "3")
                visit.VisitType = "Emergency";
            Console.Write("Enter Description : ");
            visit.Description = Console.ReadLine();
            Console.Write("Enter Doctor Name : ");
            visit.DoctorName = Console.ReadLine();
            Console.Write("Enter Duration in Minutes: ");
            visit.DurationInMinutes = int.Parse(Console.ReadLine());

            visit.Fee = _loadFee.GetFee(visit.VisitType,visit.DurationInMinutes);

            Console.WriteLine($"Fee calculated: {visit.Fee}");
            _patientVisitRepository.AddVisit(visit);

            undo.Push(JsonSerializer.Serialize(visit) + "|ADD");
            if (undo.Count > 10)
                undo = new Stack<string>(undo.ToArray()[..10]);

            redo.Clear();
            _logger.LogActivity("Add Visit", true);
        }

        public void UpdateVisit()
        {
            Console.WriteLine("Enter id of visit : ");
            int id = int.Parse(Console.ReadLine());
            PatientVisit visit = _patientVisitRepository.GetVisitById(id);
            if (visit == null)
            {
                Console.WriteLine("Visit not found in records");
                return;
            }
            undo.Push(JsonSerializer.Serialize(visit) + "|UPDATE");
            if (undo.Count > 10)
            {
                undo = new Stack<string>(undo.ToArray()[..10]);
            }
            Console.Write("Enter Patient Name : ");
            visit.PatientName = Console.ReadLine();
            Console.Write("Enter visit date and time (yyyy-MM-dd HH:mm): ");
            string inputDate = Console.ReadLine();

            DateTime visitDate;
            while (!DateTime.TryParseExact(inputDate, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out visitDate))
            {
                Console.Write("Invalid format. Please enter again (yyyy-MM-dd HH:mm): ");
                inputDate = Console.ReadLine();
            }
           
                if (_patientVisitRepository.IsVisitExistsInSameSlot(visit))
                {
                    Console.WriteLine("Warning: This patient has another visit within 30 minutes. Proceed? (Y/N)");
                    string op = Console.ReadLine();
                    if (op?.ToUpper() != "Y")
                    {
                        Console.WriteLine("Visit not added due to conflict.");
                        _logger.LogActivity("Add Visit (Conflict)", false);
                        return;
                    }
                }

            string input;
            do
            {
                Console.Write("Enter visit type (1. Consultation, 2. Follow-Up, 3. Emergency): ");
                input = Console.ReadLine();
            } while (input != "1" && input != "2" && input != "3");
            if (input == "1")
                visit.VisitType = "Consultation";
            else if (input == "2")
                visit.VisitType = "Follow-Up";
            else if (input == "3")
                visit.VisitType = "Emergency";
            visit.VisitType = Console.ReadLine();
            Console.Write("Enter description : ");
            visit.Description = Console.ReadLine();
            Console.Write("Enter Doctor Name (Optional) : ");
            visit.DoctorName = Console.ReadLine();
            Console.WriteLine("Visit Updated");
            visit.Fee= _loadFee.GetFee(visit.VisitType,visit.DurationInMinutes);
            _patientVisitRepository.UpdateVisit(visit);
            _logger.LogActivity("UpdatedVisit",true);
        }
        public void DeleteVisit()
        {
            Console.Write("Enter Visit ID to delete: ");
            int id = int.Parse(Console.ReadLine());
            var visit = _patientVisitRepository.GetVisitById(id);
            if (visit == null)
            {
                Console.WriteLine(" Visit not found.");
                return;
            }
            visits.Remove(visit);
            undo.Push(JsonSerializer.Serialize(visit) + "|DELETE");
            if (undo.Count > 10)
            {
                undo = new Stack<string>(undo.ToArray()[..10]);
            }

            Console.WriteLine(" Visit deleted.");
            redo.Clear();
            _logger.LogActivity("deletedVisit",true);
        }

        public void SearchVisits()
        {
            Console.Write("Search by (Patient/Doctor/Date/Type): ");
            string key = Console.ReadLine().ToLower();
            Console.Write("Enter search value: ");
            string value = Console.ReadLine().ToLower();
            Console.WriteLine("\n---Search Results---\n");
            List<PatientVisit> visits = _patientVisitRepository.GetAllVisits().ToList();
            foreach (var v in visits)
            {
                if (key == "patient" && v.PatientName.ToLower().Contains(value) ||key == "doctor" && v.DoctorName.ToLower().Contains(value) ||key == "date" && v.VisitDate.ToString("yyyy-MM-dd") == value ||key == "type" && v.VisitType.ToLower() == value)
                {
                    Console.WriteLine($"Patient Id : {v.Id} Patient Name : {v.PatientName},VisitType {v.VisitType},Visit Date and Time : {v.VisitDate:f},Doctor Name : Dr. {v.DoctorName}");
                }
            }
            _logger.LogActivity("SearchedVisit",true);
            Console.WriteLine("---------------------------\n");
        }

        public void ShowSummary()
        {
            Console.Write("Enter id of Visit: ");
            int id=int.Parse(Console.ReadLine());
            var visit = _patientVisitRepository.GetVisitById(id);
            if (visit == null)
            {
                Console.WriteLine("Visit Not Found");
                return;
            }
            Console.WriteLine("\n---Visit Summary---\n");
            Console.Write($"Patient Name :{visit.PatientName} \nDateTime :{visit.VisitDate}\n Visit Type : {visit.VisitType}\n Visit Description : {visit.Description}\n Doctor Name : {visit.DoctorName}\n Duration :{visit.DurationInMinutes}minutes\n Visit Fee:{visit.Fee}");
        }

        public void ShowReports()
        {
            _reportGenerator.DisplayVisitReport(visits);
        }

        public void Undo()
        {
            if (undo.Count == 0)
            {
                Console.WriteLine("Nothing to undo");
                return;
            }
            string str = undo.Pop();
            string[] parts = str.Split('|');
            PatientVisit visit = JsonSerializer.Deserialize<PatientVisit>(parts[0]);
            string operation = parts[1];
            if (operation == "ADD")
            {
                _patientVisitRepository.DeleteVisit(visit.Id);
                redo.Push(str);
            }
            else if (operation == "UPDATE")
            {
                PatientVisit v = _patientVisitRepository.GetVisitById(visit.Id);
                redo.Push(JsonSerializer.Serialize(v) + "|UPDATE");
                if (v != null)
                {
                    v.PatientName = visit.PatientName;
                    v.VisitDate = visit.VisitDate;
                    v.VisitType = visit.VisitType;
                    v.Description = visit.Description;
                    v.DoctorName = visit.DoctorName;
                    v.DurationInMinutes = visit.DurationInMinutes;
                    v.Fee = visit.Fee;
                }
            }
            else if (operation == "DELETE")
            {
                _patientVisitRepository.AddVisit(visit);
                redo.Push(str);
            }
            Console.WriteLine("Undo Completed");
            _logger.LogActivity("Undo",true);
        }
        public void Redo()
        {
            if (redo.Count == 0)
            {
                Console.WriteLine("Nothing to undo");
                return;
            }
            string str = redo.Pop();
            string[] parts = str.Split('|');
            PatientVisit visit = JsonSerializer.Deserialize<PatientVisit>(parts[0]);
            string operation = parts[1];
            if (operation == "ADD")
            {
                _patientVisitRepository.AddVisit(visit);
                undo.Push(str);
            }
            else if (operation == "DELETE")
            {
               visits.Remove(visits.Find(v => v.Id == visit.Id));
                undo.Push(str);
            }
            else if (operation == "UPDATE")
            {
                PatientVisit v = _patientVisitRepository.GetVisitById(visit.Id);
                if (v != null)
                {
                    v.PatientName = visit.PatientName;
                    v.VisitDate = visit.VisitDate;
                    v.VisitType = visit.VisitType;
                    v.Description = visit.Description;
                    v.DoctorName = visit.DoctorName;
                    v.DurationInMinutes = visit.DurationInMinutes;
                    v.Fee = visit.Fee;
                }
                undo.Push(JsonSerializer.Serialize(v) + "|UPDATE");
            }
            Console.WriteLine("Redo Completed");
            _logger.LogActivity("Redo",true);
        }

        public void GenerateMockData()
        {
            List<PatientVisit> visits = new List<PatientVisit>();
            string[] patientNames = { "Ali", "Zubair", "Zohaib", "Zeeshan", "Bilal", "Khalid" };
            string[] types = { "Consultation,Follow-Up", "Emergency" };
            int[] fee = { 500,300,1000};
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
            _patientVisitRepository.SaveAllVisits(visits);
            _logger.LogActivity("MockedVisits",true);
        }

        public void FilterAndSortVisits()
        {
            Console.WriteLine("Filter by: 1. Doctor  2. Type  3. Date Range");
            string choice = Console.ReadLine();
            IEnumerable<PatientVisit> result = _patientVisitRepository.GetAllVisits();

            if (choice == "1")
            {
                Console.Write("Enter Doctor Name: ");
                string doc = Console.ReadLine().ToLower();
                result = result.Where(v => v.DoctorName.ToLower().Contains(doc));
            }
            else if (choice == "2")
            {
                Console.Write("Enter Visit Type: ");
                string type = Console.ReadLine().ToLower();
                result = result.Where(v => v.VisitType.ToLower().Contains(type));
            }
            else if (choice == "3")
            {
                Console.Write("Enter start date (yyyy-MM-dd): ");
                DateTime start = DateTime.Parse(Console.ReadLine());
                Console.Write("Enter end date (yyyy-MM-dd): ");
                DateTime end = DateTime.Parse(Console.ReadLine());
                result = result.Where(v => v.VisitDate.Date >= start.Date && v.VisitDate.Date <= end.Date);
            }

            Console.WriteLine("Sort by: 1. Date  2. Patient Name");
            string sort = Console.ReadLine();

            if (sort == "1") result = result.OrderBy(v => v.VisitDate);
            else if (sort == "2") result = result.OrderBy(v => v.PatientName);

            foreach (var v in result)
            {
                Console.WriteLine($"{v.Id}: {v.PatientName}, {v.VisitType}, {v.VisitDate}, Dr. {v.DoctorName}, Fee: {v.Fee}");
            }
            _logger.LogActivity("SortingDone", true);
        }

    }
}
