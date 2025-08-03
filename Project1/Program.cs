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
        IAuth auth=new Auth();
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
            Console.WriteLine("\nChoose an option:");
            if (role == UserRole.Admin)
            {
                Console.WriteLine("1. Add\n2. Update\n3. Delete\n4. Search\n5. Report\n6. Undo\n7. Redo\n8. Filter/Sort\n9. Display Summary\n10. Generate Mock Data\n11. Exit");
            }
            else
            {
                Console.WriteLine("1. Add\n2. Search\n3. Report\n4. Exit");
            }
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