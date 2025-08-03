using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Interfaces
{
    public interface ILogger
    {
        public void LogActivity(string action,bool success);
    }
}
