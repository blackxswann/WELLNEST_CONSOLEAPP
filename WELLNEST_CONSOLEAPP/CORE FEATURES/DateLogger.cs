using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WELLNEST_CONSOLEAPP.FILE_HANDLING;
using WELLNEST_CONSOLEAPP.MODELS;


namespace WELLNEST_CONSOLEAPP.CORE_FEATURES
{
    public class DateLogger : EntryLogger
    {
        public DateLogger(UserInformation currentUser, FileHandler fileHandler) : base(currentUser, fileHandler) { }
        public override void Execute()
        {
            string response;
            bool hasEntries = _fileHandler.CheckIfAnyEntryExists(_currentUser);

            while (true) 
            {
                Panel dateText = new Panel("                 LOG DATE                 ");
                dateText.Border = BoxBorder.Double;
                AnsiConsole.Write(dateText);

                if (!hasEntries)
                {
                    Console.WriteLine("You haven't inputted any dates yet!");
                    Console.Write("Would you like to log a date? [Y/N]: ");
                    response = Console.ReadLine();

                    if (response == "Y" || response == "y")
                    {
                        Console.Write("Input date (mm-dd-yyyy or mm/dd/yyyy): ");
                        string tempDate = Console.ReadLine();

                        if (DateTime.TryParse(tempDate, out DateTime loggedDate) && loggedDate.Date >= DateTime.Now.Date)
                        {
                            _currentUser.CurrentLoggedDate = loggedDate.Date;
                            _fileHandler.CreateAndAddEntryFileForSpecificDate(_currentUser, EntryType.Date, _currentUser.CurrentLoggedDate.ToString("MM-dd-yyyy"));
                            Console.Write("You have successfully logged the date!");
                            Console.ReadKey(true);
                            Console.Clear();
                            return; 
                        }
                        else
                        {
                            Console.Write("Invalid date! Try again!");
                            Console.ReadKey(true);
                            Console.Clear();
                        }
                    }
                    else
                    {
                        Console.Write("No date logged. Returning to previous menu...");
                        Console.ReadKey(true);
                        return; 
                    }
                }
                else
                {
                    DateTime lastLoggedDate = _fileHandler.FindRecentDate(_currentUser);
                    Console.WriteLine($"Last logged date: {lastLoggedDate.ToString("MM-dd-yyyy")}");
                    Console.Write("Do you want to log a new date? [Y/N]: ");
                    response = Console.ReadLine();

                    string format = "MM-dd-yyyy";
                    if (response == "Y" || response == "y")
                    {
                        Console.Write("Input date (mm-dd-yyyy): ");
                        string tempDate = Console.ReadLine();

                        if (DateTime.TryParseExact(tempDate, format, null, System.Globalization.DateTimeStyles.None, out DateTime loggedDate) && loggedDate.Date >= DateTime.Now.Date &&
                         loggedDate.Date > lastLoggedDate.Date)
                        {
                            _currentUser.CurrentLoggedDate = loggedDate.Date;
                            _fileHandler.CreateAndAddEntryFileForSpecificDate(_currentUser, EntryType.Date, _currentUser.CurrentLoggedDate.ToString("MM-dd-yyyy"));

                            Console.Write("You have successfully logged the date!");
                            Console.ReadKey(true);
                            Console.Clear();
                            return; 
                        }
                        else
                        {
                            Console.Write("Invalid date format or date! Try again!");
                            Console.ReadKey(true);
                            Console.Clear();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Returning to previous menu...");
                        Console.ReadKey(true);
                        Console.Clear();
                        return;
                    }
                }
            }
        }

    }
}
