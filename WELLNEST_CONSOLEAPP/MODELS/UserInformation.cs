using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WELLNEST_CONSOLEAPP.MODELS
{
    public class UserInformation
    {
        private string _fullName; 
        private string _userName;   
        private string _password;
        private DateTime _currentLoggedDate; 

        public UserInformation(string fullName, string userName, string password) 
        {
            _fullName = fullName;
            _userName = userName;
            _password = password;
            _currentLoggedDate = DateTime.MinValue; 
        }

        public string FullName { get => _fullName; set => _fullName = value; }
        public string UserName { get => _userName; set => _userName = value; }
        public string Password { get => _password; set => _password = value; }
        public DateTime CurrentLoggedDate { get => _currentLoggedDate; set => _currentLoggedDate = value; }


    }
}
