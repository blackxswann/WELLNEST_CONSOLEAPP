using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WELLNEST_CONSOLEAPP.MODELS
{
    public class Entry
    {
        private DateTime _dateLogged;
        private EntryType _entryType;
        private string _description; 

        public Entry(DateTime dateLogged, EntryType entryType, string description)
        {
            _dateLogged = dateLogged;
            _entryType = entryType;
            _description = description;
        }

        public DateTime DateLogged { get => _dateLogged; set => _dateLogged = value; }
        public EntryType EntriesType { get => _entryType; set => _entryType = value; }    
        public string Description { get => _description; set => _description = value; }
    }

    public enum EntryType
    {
        Date, Mood, Sleep, WaterIntake, Exercise
    }
}
