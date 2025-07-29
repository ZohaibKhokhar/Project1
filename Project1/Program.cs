using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;


class PatientVisit
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public DateTime VisitDate { get; set; }
    public string VisitType { get; set; } 
    public string Description { get; set; }
    public string DoctorName { get; set; }
}

class Program
{
    static string filePath = "visits.txt";
    static List<PatientVisit> visits = new List<PatientVisit>();

    public  static void Main()
    {
        LoadVisits();

        while (true)
        {
            Console.WriteLine("\n--- Patient Visit Manager ---");
            Console.WriteLine("1. Add Visit\n2. Update Visit\n3. Delete Visit\n4. Search Visits\n5. Show Reports\n6. Exit");
            Console.Write("Select option: ");
            string input = Console.ReadLine();
            Program program=new Program();
            switch (input)
            {
                case "1": program.AddVisit(); break;
                case "2": program.UpdateVisit(); break;
                case "3": program.DeleteVisit(); break;
                case "4": program.SearchVisits(); break;
                case "5": program.ShowReports(); break;
                case "6": program.SaveVisits(); return;
                default: Console.WriteLine("Invalid option!"); break;
            }
        }
    }

    public void AddVisit()
    {
        PatientVisit visit = new PatientVisit();

        visit.Id = visits.Any() ? visits.Max(v => v.Id) + 1 : 1;
        Console.Write("Patient Name: ");
        visit.PatientName = Console.ReadLine();
        Console.Write("Visit Date (yyyy-mm-dd): ");
        visit.VisitDate = DateTime.Parse(Console.ReadLine());
        Console.Write("Visit Type (Consultation/Follow-up/Emergency): ");
        visit.VisitType = Console.ReadLine();
        Console.Write("Description: ");
        visit.Description = Console.ReadLine();
        Console.Write("Doctor Name (optional): ");
        visit.DoctorName = Console.ReadLine();

        visits.Add(visit);
        Console.WriteLine(" Visit added!");
        SaveVisits();
    }

    public void UpdateVisit()
    {
        Console.Write("Enter Visit ID to update: ");
        int id = int.Parse(Console.ReadLine());
        var visit = visits.Find(v => v.Id == id);
        if (visit == null) 
        { 
            Console.WriteLine(" Visit not found.");
            return;
        }

        Console.Write("New Patient Name: ");
        visit.PatientName = Console.ReadLine();
        Console.Write("New Visit Date (yyyy-mm-dd): ");
        visit.VisitDate = DateTime.Parse(Console.ReadLine());
        Console.Write("New Visit Type: ");
        visit.VisitType = Console.ReadLine();
        Console.Write("New Description: ");
        visit.Description = Console.ReadLine();
        Console.Write("New Doctor Name: ");
        visit.DoctorName = Console.ReadLine();

        Console.WriteLine(" Visit updated.");
        SaveVisits();
    }

    public void DeleteVisit()
    {
        Console.Write("Enter Visit ID to delete: ");
        int id = int.Parse(Console.ReadLine());
        var visit = visits.Find(v => v.Id == id);
        if (visit == null) { Console.WriteLine(" Visit not found."); return; }

        visits.Remove(visit);

        Console.WriteLine(" Visit deleted.");
        SaveVisits();
    }

    public void SearchVisits()
    {
        Console.Write("Search by (Patient/Doctor/Date/Type): ");
        string key = Console.ReadLine().ToLower();
        Console.Write("Enter search value: ");
        string value = Console.ReadLine().ToLower();

        foreach (var v in visits)
        {
            if ((key == "patient" && v.PatientName.ToLower().Contains(value)) ||
                (key == "doctor" && v.DoctorName.ToLower().Contains(value)) ||
                (key == "date" && v.VisitDate.ToString("yyyy-MM-dd") == value) ||
                (key == "type" && v.VisitType.ToLower() == value))
            {
                Console.WriteLine($"{v.Id}: {v.PatientName}, {v.VisitType}, {v.VisitDate:yyyy-MM-dd}, Dr. {v.DoctorName}");
            }
        }
    }

    public void ShowReports()
    {
        Console.WriteLine("Visit Count by Type:");
        Dictionary<string, int> typeCount = new();

        foreach (var v in visits)
        {
            if (!typeCount.ContainsKey(v.VisitType)) 
                typeCount[v.VisitType] = 0;
            typeCount[v.VisitType]++;
        }

        foreach (var kvp in typeCount)
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
    }

  
  
    static void LoadVisits()
    {
        if (!File.Exists(filePath)) return;

        string[] lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            var visit = JsonSerializer.Deserialize<PatientVisit>(line);
            visits.Add(visit);
        }
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
