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
    static Stack<string> undo = new Stack<string>();
    static Stack<string> redo = new Stack<string>();
    public static void Main()
    {
        Program program = new Program();
        program.GetAllVisits();

        while (true)
        {
            Console.WriteLine("\n--- Patient Visit Manager ---");
            Console.WriteLine("1. Add Visit\n2. Update Visit\n3. Delete Visit\n4. Search Visits\n5. Show Reports\n6. Exit");
            Console.Write("Select option: ");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1": program.AddVisit(); break;
                case "2": program.UpdateVisit(); break;
                case "3": program.DeleteVisit(); break;
                case "4": program.SearchVisits(); break;
                case "5": program.ShowReports(); break;
                case "6": program.SaveAllVisits(); return;
                default: Console.WriteLine("Invalid option!"); break;
            }
        }
    }

    public void AddVisit()
    {
        PatientVisit visit = new PatientVisit();
        Console.WriteLine("Enter Patient Name : ");
        visit.PatientName = Console.ReadLine();
        Console.WriteLine("Enter Visit Date in format yyyy-mm-dd: ");
        visit.VisitDate = DateTime.Parse(Console.ReadLine());
        Console.WriteLine("Enter Visit Type : ");
        visit.VisitType = Console.ReadLine();
        Console.WriteLine("Enter Description : ");
        visit.Description = Console.ReadLine();
        Console.WriteLine("Enter Doctor Name : ");
        visit.DoctorName = Console.ReadLine();
        visit.Id = visits.Count > 0 ? visits.Max(v => v.Id)+1 : 1;
        visits.Add(visit);
        SaveAllVisits();

        undo.Push(JsonSerializer.Serialize(visit)+"|ADD");
        if (undo.Count > 10)
        {
            undo = new Stack<string>(undo.ToArray()[..10]);
        }
    }

    public void UpdateVisit()
    {
        Console.WriteLine("Enter id of visit : ");
        int id = int.Parse(Console.ReadLine());
        PatientVisit visit = visits.Find(v => v.Id == id);
        if (visit == null)
        {
            Console.WriteLine("Visit not found in records");
            return;
        }
        Console.Write("Enter Patient Name : ");
        visit.PatientName = Console.ReadLine();
        Console.Write("Enter Visit Date : ");
        visit.VisitDate = DateTime.Parse(Console.ReadLine());
        Console.Write("Enter visit type : ");
        visit.VisitType = Console.ReadLine();
        Console.Write("Enter description : ");
        visit.Description = Console.ReadLine();
        Console.Write("Enter Doctor Name (Optional) : ");
        visit.Description = Console.ReadLine();
        Console.WriteLine("Visit Updated");
        undo.Push(JsonSerializer.Serialize(visit)+"|UPDATE");
        if (undo.Count > 10)
        {
            undo = new Stack<string>(undo.ToArray()[..10]);
        }
        SaveAllVisits();

    }
    public void DeleteVisit()
    {
        Console.Write("Enter Visit ID to delete: ");
        int id = int.Parse(Console.ReadLine());
        var visit = visits.Find(v => v.Id == id);
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
        SaveAllVisits();
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
        Dictionary<string, int> dic = new Dictionary<string, int>();

        foreach (var v in visits)
        {
            if (!dic.ContainsKey(v.VisitType))
                dic[v.VisitType] = 0;
            dic[v.VisitType]++;
        }

        foreach (var d in dic)
            Console.WriteLine($"{d.Key}: {d.Value}");
    }

    //public void Undo()
    //{
    //    if (undo.Count == 0)
    //    {
    //        Console.WriteLine("Nothing to undo");
    //        return;
    //    }
    //    string str = undo.Pop();
    //    string[] parts = str.Split('|');
    //    PatientVisit visit = JsonSerializer.Deserialize<PatientVisit>(parts[0]);
    //    string operation = parts[1];
    //    if (operation == "ADD")
    //    {
    //        visits.redo();
    //    }
    //}



    public void GetAllVisits()
    {
        if (!File.Exists(filePath)) return;

        string[] lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            var visit = JsonSerializer.Deserialize<PatientVisit>(line);
            visits.Add(visit);
        }
    }

    public void SaveAllVisits()
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
