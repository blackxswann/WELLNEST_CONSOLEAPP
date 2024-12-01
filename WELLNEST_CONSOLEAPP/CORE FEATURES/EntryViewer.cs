using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WELLNEST_CONSOLEAPP.FILE_HANDLING;
using WELLNEST_CONSOLEAPP.MODELS;

namespace WELLNEST_CONSOLEAPP.CORE_FEATURES
{
    public class EntryViewer
    {
        private FileHandler _fileHandler;
        private UserInformation _user; 

        public EntryViewer(FileHandler fileHandler, UserInformation user)
        {
            _fileHandler = fileHandler;
            _user = user;
        }

        public void ShowEntryViewerOptionDisplay()
        {
            int selectedIndex = 0;
            string[] options = new string[]
            {
        "VIEW ENTRY BY DATE",
        "DELETE ALL ENTRIES BY DATE",
        "GO BACK TO MAIN MENU"
            };

            while (true)
            {
                Console.Clear();
                DisplayOptions(options, selectedIndex, "ENTRY VIEWER");

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (selectedIndex > 0) selectedIndex--;
                        break;

                    case ConsoleKey.DownArrow:
                        if (selectedIndex < options.Length - 1) selectedIndex++;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0:
                                Console.Clear();
                                ViewEntryByDate();
                                break;

                            case 1:
                                Console.Clear();
                                DeleteEntryByDate();
                                break;

                            case 2:
                                Console.Clear();
                                return;
                        }
                        break;
                }
            }
        }



        private void ViewEntryByDate()
        {
            List<string> data = new List<string>();

            Console.WriteLine("\tVIEW ENTRY BY DATE\n");
            Console.Write("Input date of the entry to be viewed (mm-dd-yyyy or mm/dd/yyyy): ");
            string tempInput = Console.ReadLine();

            string format = "MM-dd-yyyy";

            if (DateTime.TryParseExact(tempInput, format, null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                data = _fileHandler.ViewAndDeleteFileBySpecificDate(date, 0, _user);
                if (data == null)
                {
                    Console.ReadKey(true);
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine($"ENTRIES FOR {date:MM-dd-yyyy}");

                    string moodEntry = "Mood entry: No entry";
                    string sleepEntry = "Sleep entry: No entry";
                    string waterEntry = "Water intake entry: No entry";
                    string exerciseEntry = "Exercise entry: No entry";

                    foreach (string entry in data)
                    {
                        string[] entryParts = entry.Split(',');

                        if (entryParts.Length >= 3)
                        {
                            string entryType = entryParts[1];
                            string description = entryParts[2];

                            switch (entryType)
                            {
                                case "Mood":
                                    if (entryParts[2] == "VeryUnhappy")
                                    {
                                        moodEntry = $"Mood entry: Very Unhappy";
                                    }
                                    else if (entryParts[2] == "VeryHappy")
                                    {
                                        moodEntry = $"Mood entry: Very Happy";
                                    }
                                    else
                                    {
                                        moodEntry = $"Mood entry: {description}";
                                    }
                                    break;
                                case "Sleep":
                                    sleepEntry = $"Sleep entry: {description} hours";
                                    break;
                                case "WaterIntake":
                                    waterEntry = $"Water intake entry: {description} glasses";
                                    break;
                                case "Exercise":
                                    exerciseEntry = $"Exercise entry: {description} minutes";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    Console.WriteLine(moodEntry);
                    Console.WriteLine(sleepEntry);
                    Console.WriteLine(waterEntry);
                    Console.WriteLine(exerciseEntry);
                    Console.ReadKey(true); 
                }
            }
            else
            {
                Console.WriteLine("Invalid input! Try again.");
                Console.ReadKey(true);
                Console.Clear();
                return;
            }
        }

        private void DeleteEntryByDate()
        {
            List<string> data = new List<string>();

            Console.WriteLine("\tDELETE ENTRY BY DATE\n");
            Console.Write("Input date of the entry to be deleted (mm-dd-yyyy or mm/dd/yyyy): ");
            string tempInput = Console.ReadLine();

            string format = "MM-dd-yyyy";

            if (DateTime.TryParseExact(tempInput, format, null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                data = _fileHandler.ViewAndDeleteFileBySpecificDate(date, 1, _user);
                if (data == null)
                {
                    Console.ReadKey(true);
                    Console.Clear();
                }
                else
                {
                    Console.Write($"You have successfully deleted all entries for {date:MM-dd-yyyy}.");
                    Console.ReadKey(true);
                    Console.Clear(); 
                }
                
            }
            else
            {
                Console.WriteLine("Invalid input! Try again.");
                Console.ReadKey(true);
                Console.Clear();
                return;
            }
        }


        void DisplayOptions(string[] options, int selectedIndex, string text)
        {
            int consoleWidth = Console.WindowWidth;
            int consoleHeight = Console.WindowHeight;

            int totalVerticalPadding = consoleHeight - options.Length - 3;
            int topPadding = totalVerticalPadding / 2;

            for (int j = 0; j < topPadding; j++)
            {
                Console.WriteLine();
            }

            int textLeftPadding = (consoleWidth - text.Length) / 2;
            Console.WriteLine(new string(' ', textLeftPadding) + text);
            Console.WriteLine();

            for (int i = 0; i < options.Length; i++)
            {
                string currentOption = options[i];
                int leftPadding = (consoleWidth - currentOption.Length) / 2;

                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine(new string(' ', leftPadding) + currentOption + new string(' ', leftPadding));
            }

            Console.ResetColor();
        }
    }
}
