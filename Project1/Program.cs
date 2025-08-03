using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using Project1.Helper;
using Project1.Models;
using Project1.Interfaces;
using Project1.Services;
using Project1.Repositories;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("===========================================");
        Console.WriteLine("  Welcome to Patient Visit Management System");
        Console.WriteLine("===========================================\n");
        Console.WriteLine("Please login to continue.\n");
        IAuth auth =new Auth();
        var (role, isAuthenticated) = auth.Authenticate();
        if (!isAuthenticated)
        {
            Console.WriteLine("Invalid login.");
            return;
        }
        ILogger logger = new Logger();
        IGenerateReport generateReport = new GenerateReport();
        IPatientVisitRepository patientVisitRepository = new PatientVisitRepository();
        ILoadConsultationFee loadFee = new LoadConsultationFee();
        IPatientVisitManager manager = new PatientVisitManager(logger,generateReport,patientVisitRepository,loadFee);

        while (true)
        {
            Console.WriteLine("\n========== Main Menu ==========");

            if (role == UserRole.Admin)
            {
                Console.WriteLine("1.  Add New Visit");
                Console.WriteLine("2.  Update Visit");
                Console.WriteLine("3.  Delete Visit");
                Console.WriteLine("4.  Search Visits");
                Console.WriteLine("5.  Generate Report");
                Console.WriteLine("6.  Undo Last Action");
                Console.WriteLine("7.  Redo Last Action");
                Console.WriteLine("8.  Filter / Sort Visits");
                Console.WriteLine("9.  Display Summary");
                Console.WriteLine("10. Generate Mock Data");
                Console.WriteLine("11. Exit");
            }
            else
            {
                Console.WriteLine("1.  Add New Visit");
                Console.WriteLine("2.  Search Visits");
                Console.WriteLine("3.  Generate Report");
                Console.WriteLine("4.  Exit");
            }

            Console.WriteLine("================================");
            int maxOption = role == UserRole.Admin ? 11 : 4;
            Console.Write($"Select an option (1 - {maxOption}): ");

            Console.Write("Enter your choice : ");
            string choice = Console.ReadLine();

            if (role == UserRole.Admin)
            {
                switch (choice)
                {
                    case "1":
                        manager.AddVisit();
                        break;
                    case "2":
                        manager.UpdateVisit();
                        break;
                    case "3":
                        manager.DeleteVisit();
                        break;
                    case "4":
                        manager.SearchVisits();
                        break;
                    case "5":
                        manager.ShowReports();
                        break;
                    case "6":
                        manager.Undo();
                        break;
                    case "7":
                        manager.Redo();
                        break;
                    case "8":
                        manager.FilterAndSortVisits();
                        break;
                    case "9":
                        manager.ShowSummary();
                        break;
                    case "10":
                        manager.GenerateMockData();
                        break;
                    case "11":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            else
            {
                switch (choice)
                {
                    case "1":
                        manager.AddVisit();
                        break;
                    case "2":
                        manager.SearchVisits();
                        break;
                    case "3":
                        manager.ShowReports();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}