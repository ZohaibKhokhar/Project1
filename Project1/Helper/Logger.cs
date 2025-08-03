using Project1.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Helper
{
    public class Logger:ILogger
    {
        public void LogActivity(string action, bool success)
        {
            string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {action} | {(success ? "Success" : "Failure")}";
            File.AppendAllText("activityLogging.txt", logLine + Environment.NewLine);
        }
    }
}
