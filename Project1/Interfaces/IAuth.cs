using Project1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project1.Services.Auth;

namespace Project1.Interfaces
{
    public interface IAuth
    {
        public (UserRole, bool) Authenticate();
    }
}
