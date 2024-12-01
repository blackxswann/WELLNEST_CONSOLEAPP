using System;
using WELLNEST_CONSOLEAPP.FILE_HANDLING;
using WELLNEST_CONSOLEAPP.MODELS;

namespace WELLNEST_CONSOLEAPP.CORE_FEATURES
{
    public class ExerciseLogger : EntryLogger
    {
        public ExerciseLogger(UserInformation currentUser, FileHandler fileHandler) : base(currentUser, fileHandler) { }

        public override void Execute()
        {
            while (true)
            {
                Console.WriteLine("\tLOG EXERCISE:\n");

                string response;
                bool hasEntries = _fileHandler.CheckIfAnyEntryExists(_currentUser);

                if (!hasEntries)
                {
                    Console.WriteLine("You haven't logged any entries yet!");
                    Console.Write("Please log ta date first before adding an exercise entry. Press any key to return.");
                    Console.ReadKey(true);
                    Console.Clear();
                    return;
                }

                DateTime lastLoggedDate = _fileHandler.FindRecentDate(_currentUser);
                string[] tempdata = _fileHandler.LoadFileBySpecificDate(lastLoggedDate, EntryType.Exercise);

                if (tempdata.Length == 3 && string.IsNullOrWhiteSpace(tempdata[0]))
                {
                    Console.WriteLine($"Current logged date: {lastLoggedDate.ToString("MM-dd-yyyy")}, Exercise: No inputs yet!");
                    Console.Write("Would you like to create an exercise entry? [Y/N]: ");
                    response = Console.ReadLine();

                    if (response == "Y" || response == "y")
                    {
                        LogExercise(lastLoggedDate, false);
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
                    Console.WriteLine($"Last logged exercise date: {lastLoggedDate.ToString("MM-dd-yyyy")}, Exercise: {tempdata[2]}");
                    Console.Write("Do you want to edit your exercise entry? [Y/N]: ");
                    response = Console.ReadLine();

                    if (response == "Y" || response == "y")
                    {
                        LogExercise(lastLoggedDate, true);
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

        private void LogExercise(DateTime loggedDate, bool isEditing)
        { 

            Console.Write("Please enter the duration of your exercise (in minutes): ");
            string tempDuration = Console.ReadLine();

            if (int.TryParse(tempDuration, out int duration) && duration > 0)
            {
                string exerciseLog = $"{duration}";

                if (isEditing)
                {
                    _fileHandler.ReplaceEntryForSpecificDate(_currentUser, EntryType.Exercise, exerciseLog);
                    Console.WriteLine("Your exercise entry has been successfully updated!");
                }
                else
                {
                    _fileHandler.CreateAndAddEntryFileForSpecificDate(_currentUser, EntryType.Exercise, exerciseLog);
                    Console.WriteLine("You have successfully logged an exercise entry!");
                }

                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Invalid duration input! Please enter a positive number.");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
    }
}
