using Spectre.Console;
using System;
using WELLNEST_CONSOLEAPP.FILE_HANDLING;
using WELLNEST_CONSOLEAPP.MODELS;

namespace WELLNEST_CONSOLEAPP.CORE_FEATURES
{
    public class SleepLogger : EntryLogger
    {
        public SleepLogger(UserInformation currentUser, FileHandler fileHandler) : base(currentUser, fileHandler) { }

        public override void Execute()
        {
            while (true)
            {
                Panel sleepHeader = new Panel("                LOG SLEEP                ");
                sleepHeader.Border = BoxBorder.Double;
                AnsiConsole.Write(sleepHeader);

                string response;
                bool hasEntries = _fileHandler.CheckIfAnyEntryExists(_currentUser);

                if (!hasEntries)
                {
                    Console.WriteLine("You haven't logged any entries yet!");
                    Console.Write("Please log the date first before adding a sleep entry. Press any key to return.");
                    Console.ReadKey(true);
                    Console.Clear();
                    return;
                }

                DateTime lastLoggedDate = _fileHandler.FindRecentDate(_currentUser);
                string[] tempdata = _fileHandler.LoadFileBySpecificDate(lastLoggedDate, EntryType.Sleep);

                if (tempdata.Length == 3 && string.IsNullOrWhiteSpace(tempdata[0]))
                {
                    Console.WriteLine($"Current logged date: {lastLoggedDate.ToString("MM-dd-yyyy")}, Sleep: No inputs yet!");
                    Console.Write("Would you like to create a sleep entry? [Y/N]: ");
                    response = Console.ReadLine();

                    if (response == "Y" || response == "y")
                    {
                        LogSleep(lastLoggedDate);
                        return;
                    }
                    else if (response == "N" || response == "n")
                    {
                        Console.Clear();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input! Try again.");
                        Console.ReadKey(true);
                        Console.Clear();
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"Last logged sleep date: {lastLoggedDate.ToString("MM-dd-yyyy")}, Sleep: {tempdata[2]}");
                    Console.Write("Do you want to edit your sleep entry? [Y/N]: ");
                    response = Console.ReadLine();

                    if (response == "Y" || response == "y")
                    {
                        LogSleep(lastLoggedDate, true);
                        return;
                    }
                    else if (response == "N" || response == "n")
                    {
                        Console.Clear();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input! Try again.");
                        Console.ReadKey(true);
                        Console.Clear();
                        return;
                    }
                }
            }
        }

        private void LogSleep(DateTime loggedDate, bool isEditing = false)
        {
            Console.Write("Please enter the number of hours you slept last night: ");
            string tempHours = Console.ReadLine();

            if (int.TryParse(tempHours, out int hours) && hours > 0)
            {
                string sleepLog = $"{hours}";

                if (isEditing)
                {
                    _fileHandler.ReplaceEntryForSpecificDate(_currentUser, EntryType.Sleep, sleepLog);
                    Console.WriteLine("Your sleep entry has been successfully updated!");
                }
                else
                {
                    _fileHandler.CreateAndAddEntryFileForSpecificDate(_currentUser, EntryType.Sleep, sleepLog);
                    Console.WriteLine("You have successfully logged your sleep!");
                }

                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Invalid number of hours! Please enter a positive number.");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
    }
}
