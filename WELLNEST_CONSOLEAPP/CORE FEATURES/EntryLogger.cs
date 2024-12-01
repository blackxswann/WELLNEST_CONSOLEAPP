using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WELLNEST_CONSOLEAPP.MODELS;
using WELLNEST_CONSOLEAPP.FILE_HANDLING;

namespace WELLNEST_CONSOLEAPP.CORE_FEATURES
{
    public abstract class EntryLogger
    {
        protected UserInformation _currentUser;
        protected FileHandler _fileHandler;

        public EntryLogger(UserInformation currentUser, FileHandler fileHandler)
        {
            _currentUser = currentUser;
            _fileHandler = fileHandler;
        }  
        public abstract void Execute(); 
    }
}
