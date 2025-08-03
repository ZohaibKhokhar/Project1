using Project1.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project1.Helper
{
    public class LoadConsultationFee:ILoadConsultationFee
    {
        static Dictionary<string, decimal> feeRules = new Dictionary<string, decimal>();

        public LoadConsultationFee()
        {
            LoadFee();
        }

        public void LoadFee()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fees.json");

            try
            {
                if (!File.Exists(path))
                {
                    var defaultFees = new Dictionary<string, decimal>{
                { "Consultation", 500 },
                { "Follow-Up", 300 },
                { "Emergency", 1000 }
                };

                    string defaultJson = JsonSerializer.Serialize(defaultFees, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(path, defaultJson);
                    Console.WriteLine("Created default fees.json at: " + path);
                }
                string json = File.ReadAllText(path);
                feeRules = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load fees.json: " + ex.Message);
                feeRules = new Dictionary<string, decimal>();
            }
        }

        public decimal GetFee(string visitType, int duration)
        {
            if (feeRules.TryGetValue(visitType, out decimal fee))
            {
                return (duration<=30?fee:fee*(duration/30));
            }
            else
            { 
                return 0;
            }
        }
    }
}
