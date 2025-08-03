using Project1.Interfaces;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Services
{
    public enum UserRole { Admin, Receptionist }
    public class Auth:IAuth
    {
        public (UserRole, bool) Authenticate()
        {
            User user = new User();
            Console.Write("Enter Your UserName: ");
            user.Username = Console.ReadLine();
            Console.Write("Enter Your Password: ");
            user.Password = Console.ReadLine();

            if (user.Username == "admin" && user.Password == "admin123")
                return (UserRole.Admin, true);
            else if (user.Username == "ali" && user.Password == "1234")
                return (UserRole.Receptionist, true);
            else
                return (UserRole.Admin, false);
        }
    }
}
