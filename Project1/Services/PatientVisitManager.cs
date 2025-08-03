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
        private readonly IMockData _mockData;
        static Stack<string> undo = new Stack<string>();
        static Stack<string> redo = new Stack<string>();

        public PatientVisitManager(ILogger logger,IGenerateReport reportGenerator,IPatientVisitRepository patientVisitRepository,ILoadConsultationFee loadFee,IMockData mockData)
        {
            _logger=logger;
            _reportGenerator=reportGenerator;
            _patientVisitRepository=patientVisitRepository;
            _loadFee=loadFee;
            _mockData=mockData;
        }
        public void AddVisit()
        {
            PatientVisit visit = new PatientVisit();
            Console.WriteLine("\n========== Add New Patient Visit ==========");

            Console.Write("Patient Name                : ");
            visit.PatientName = Console.ReadLine();

            Console.Write("Visit Date & Time (yyyy-MM-dd HH:mm): ");
            string inputDate = Console.ReadLine();

            DateTime visitDate;
            while (!DateTime.TryParseExact(inputDate, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out visitDate))
            {
                Console.Write("Invalid format. Re-enter (yyyy-MM-dd HH:mm): ");
                inputDate = Console.ReadLine();
            }

            visit.VisitDate = visitDate;

            if (_patientVisitRepository.IsVisitExistsInSameSlot(visit))
            {
                Console.WriteLine("Warning: Another visit exists within 30 minutes.");
                Console.Write("Do you still want to proceed? (Y/N): ");
                string op = Console.ReadLine();
                if (op?.ToUpper() != "Y")
                {
                    Console.WriteLine("Visit not added due to time conflict.");
                    _logger.LogActivity("Add Visit (Conflict)", false);
                    return;
                }
            }

            string input;
            do
            {
                Console.Write("Visit Type (1. Consultation, 2. Follow-Up, 3. Emergency): ");
                input = Console.ReadLine();
            } while (input != "1" && input != "2" && input != "3");

            visit.VisitType = input switch
            {
                "1" => "Consultation",
                "2" => "Follow-Up",
                "3" => "Emergency",
                _ => ""
            };

            Console.Write("Description                : ");
            visit.Description = Console.ReadLine();

            Console.Write("Doctor Name                : ");
            visit.DoctorName = Console.ReadLine();

            Console.Write("Duration (minutes)         : ");
            visit.DurationInMinutes = int.Parse(Console.ReadLine());

            visit.Fee = _loadFee.GetFee(visit.VisitType, visit.DurationInMinutes);

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
            Console.WriteLine("\n========== Update Patient Visit ==========");

            Console.Write("Visit ID to update         : ");
            int id = int.Parse(Console.ReadLine());

            PatientVisit visit = _patientVisitRepository.GetVisitById(id);
            if (visit == null)
            {
                Console.WriteLine("Visit not found.");
                return;
            }

            undo.Push(JsonSerializer.Serialize(visit) + "|UPDATE");
            if (undo.Count > 10)
                undo = new Stack<string>(undo.ToArray()[..10]);

            Console.Write("Patient Name                : ");
            visit.PatientName = Console.ReadLine();

            Console.Write("Visit Date & Time (yyyy-MM-dd HH:mm): ");
            string inputDate = Console.ReadLine();

            DateTime visitDate;
            while (!DateTime.TryParseExact(inputDate, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out visitDate))
            {
                Console.Write("Invalid format. Re-enter (yyyy-MM-dd HH:mm): ");
                inputDate = Console.ReadLine();
            }

            visit.VisitDate = visitDate;

            if (_patientVisitRepository.IsVisitExistsInSameSlot(visit))
            {
                Console.WriteLine("Warning: Another visit exists within 30 minutes.");
                Console.Write("Do you still want to proceed? (Y/N): ");
                string op = Console.ReadLine();
                if (op?.ToUpper() != "Y")
                {
                    Console.WriteLine("Visit not updated due to time conflict.");
                    _logger.LogActivity("Update Visit (Conflict)", false);
                    return;
                }
            }

            string input;
            do
            {
                Console.Write("Visit Type (1. Consultation, 2. Follow-Up, 3. Emergency): ");
                input = Console.ReadLine();
            } while (input != "1" && input != "2" && input != "3");

            visit.VisitType = input switch
            {
                "1" => "Consultation",
                "2" => "Follow-Up",
                "3" => "Emergency",
                _ => ""
            };

            Console.Write("Description                : ");
            visit.Description = Console.ReadLine();

            Console.Write("Doctor Name (Optional)     : ");
            visit.DoctorName = Console.ReadLine();

            Console.WriteLine("Visit updated successfully.");

            visit.Fee = _loadFee.GetFee(visit.VisitType, visit.DurationInMinutes);
            _patientVisitRepository.UpdateVisit(visit);
            _logger.LogActivity("UpdatedVisit", true);
        }

        public void DeleteVisit()
        {
            Console.WriteLine("\n========== Delete Patient Visit ==========");

            Console.Write("Visit ID to delete         : ");
            int id = int.Parse(Console.ReadLine());

            var visit = _patientVisitRepository.GetVisitById(id);
            if (visit == null)
            {
                Console.WriteLine("Visit not found.");
                return;
            }

            _patientVisitRepository.DeleteVisit(id);
            undo.Push(JsonSerializer.Serialize(visit) + "|DELETE");

            if (undo.Count > 10)
                undo = new Stack<string>(undo.ToArray()[..10]);

            redo.Clear();
            Console.WriteLine("Visit deleted successfully.");
            _logger.LogActivity("deletedVisit", true);
        }

        public void SearchVisits()
        {
            Console.WriteLine("\n========== Search Patient Visits ==========");

            Console.Write("Search by (Patient/Doctor/Date/Type): ");
            string key = Console.ReadLine().ToLower();

            Console.Write("Enter search value         : ");
            string value = Console.ReadLine().ToLower();

            Console.WriteLine("\n--- Search Results ---\n");

            List<PatientVisit> visits = _patientVisitRepository.GetAllVisits().ToList();
            foreach (var v in visits)
            {
                if (key == "patient" && v.PatientName.ToLower().Contains(value) ||
                    key == "doctor" && v.DoctorName.ToLower().Contains(value) ||
                    key == "date" && v.VisitDate.ToString("yyyy-MM-dd") == value ||
                    key == "type" && v.VisitType.ToLower() == value)
                {
                    Console.WriteLine($"Patient ID    : {v.Id}");
                    Console.WriteLine($"Patient Name  : {v.PatientName}");
                    Console.WriteLine($"Visit Type    : {v.VisitType}");
                    Console.WriteLine($"Visit Date    : {v.VisitDate:f}");
                    Console.WriteLine($"Doctor Name   : Dr. {v.DoctorName}");
                    Console.WriteLine("-------------------------------------------");
                }
            }

            _logger.LogActivity("SearchedVisit", true);
        }

        public void ShowSummary()
        {
            Console.Write("Enter id of Visit: ");
            int id = int.Parse(Console.ReadLine());
            _reportGenerator.DisplaySummary(_patientVisitRepository.GetVisitById(id));
        }

        public void ShowReports()
        {
            _reportGenerator.DisplayVisitReport(_patientVisitRepository.GetAllVisits());
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

            Console.WriteLine("Undo operation completed successfully.\n");
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
               _patientVisitRepository.DeleteVisit(visit.Id);
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
            Console.WriteLine("Redo operation completed successfully.\n");
            _logger.LogActivity("Redo",true);
        }

        public void GenerateMockData()
        {
            List<PatientVisit> visits = _mockData.GenerateMockData();
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
