using Spectre.Console;
using System;
using WELLNEST_CONSOLEAPP.FILE_HANDLING;
using WELLNEST_CONSOLEAPP.MODELS;

namespace WELLNEST_CONSOLEAPP.CORE_FEATURES
{
    public class WaterIntakeLogger : EntryLogger
    {
        public WaterIntakeLogger(UserInformation currentUser, FileHandler fileHandler) : base(currentUser, fileHandler) { }
        public override void Execute()
        {
            while (true)
            {
                Panel waterHeader = new Panel("                LOG WATER INTAKE              ");
                waterHeader.Border = BoxBorder.Double;
                AnsiConsole.Write(waterHeader);

                string response;
                bool hasEntries = _fileHandler.CheckIfAnyEntryExists(_currentUser);

                if (!hasEntries)
                {
                    Console.WriteLine("You haven't logged any entries yet!");
                    Console.Write("Please log the date first before adding a water intake entry. Press any key to return.");
                    Console.ReadKey(true);
                    Console.Clear();
                    return;
                }

                DateTime lastLoggedDate = _fileHandler.FindRecentDate(_currentUser);
                string[] tempdata = _fileHandler.LoadFileBySpecificDate(lastLoggedDate, EntryType.WaterIntake);

                if (tempdata.Length == 3 && string.IsNullOrWhiteSpace(tempdata[0]))
                {
                    Console.WriteLine($"Current logged date: {lastLoggedDate.ToString("MM-dd-yyyy")}, Water Intake: No inputs yet!");
                    Console.Write("Would you like to create a water intake entry? [Y/N]: ");
                    response = Console.ReadLine();

                    if (response == "Y" || response == "y")
                    {
                        LogWaterIntake(lastLoggedDate);
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
                    Console.WriteLine($"Last logged water intake date: {lastLoggedDate.ToString("MM-dd-yyyy")}, Water Intake: {tempdata[2]}");
                    Console.Write("Do you want to edit your water intake entry? [Y/N]: ");
                    response = Console.ReadLine();

                    if (response == "Y" || response == "y")
                    {
                        LogWaterIntake(lastLoggedDate, true);
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

        private void LogWaterIntake(DateTime loggedDate, bool isEditing = false)
        {
            Console.Write("Please enter the number of glasses of water you drank today: ");
            string tempGlasses = Console.ReadLine();

            if (int.TryParse(tempGlasses, out int glasses) && glasses > 0)
            {
                string waterIntakeLog = $"{glasses}";

                if (isEditing)
                {
                    _fileHandler.ReplaceEntryForSpecificDate(_currentUser, EntryType.WaterIntake, waterIntakeLog);
                    Console.WriteLine("Your water intake entry has been successfully updated!");
                }
                else
                {
                    _fileHandler.CreateAndAddEntryFileForSpecificDate(_currentUser, EntryType.WaterIntake, waterIntakeLog);
                    Console.WriteLine("You have successfully logged your water intake!");
                }

                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Invalid number of glasses! Please enter a positive number.");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
    }
}
