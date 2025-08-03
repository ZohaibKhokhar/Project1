using Project1.Helper;
using Project1.Models;
using Project1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Interfaces
{
    public interface IMockData
    {
        public List<PatientVisit> GenerateMockData();
    }
}
