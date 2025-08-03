using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Interfaces
{
    public interface ILoadConsultationFee
    {
        public void LoadFee();
        public decimal GetFee(string visitType,int duration);
    }
}
