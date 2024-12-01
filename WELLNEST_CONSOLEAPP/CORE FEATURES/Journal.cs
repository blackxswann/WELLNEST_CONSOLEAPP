using System;
using WELLNEST_CONSOLEAPP.FILE_HANDLING;
using WELLNEST_CONSOLEAPP.MODELS;

namespace WELLNEST_CONSOLEAPP.CORE_FEATURES
{
    public class Journal
    {
        FileHandler _fileHandler;
        UserInformation _currentUser;
        DateTime _currentDate;
        public Journal(FileHandler fileHandler, UserInformation currentUser)
        {
            _fileHandler = fileHandler;
            _currentUser = currentUser;
            _currentDate = _fileHandler.FindRecentDate(_currentUser);
        }

        public void JournalDisplay()
        {
            string[] options = new string[]
            {
                "Add journal entry",
                "View journal entry",
                "Remove journal entry",
                "Go back to main menu"
            };

            int selectedIndex = 0; 

            while (true)
            {
                Console.Clear();
                DisplayOptions(options, selectedIndex, "-> JOURNAL <-");  
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
                                AddJournalEntry();
                                break;
                            case 1: 
                                Console.Clear();
                                ViewJournalEntry();
                                break;
                            case 2:
                                Console.Clear();
                                RemoveJournalEntry();
                                break;
                            case 3:
                                Console.Clear();
                                return;
                                break;
                        }
                        break;
                    default:
                        break;
                }
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
        private void AddJournalEntry()
        {
            if (_currentDate == null)
            {
                Console.WriteLine("Please input date first before logging an entry!");
                return;
            }

            string existingEntry = _fileHandler.ViewJournalEntry(_currentDate);

            if (existingEntry.Contains("No journal entry exists"))
            {
                Console.WriteLine($"Write journal entry for {_currentDate:MM-dd-yyyy}");
                Console.Write("Entry: ");
                string entry = Console.ReadLine();

                _fileHandler.SaveJournalEntry(_currentUser.CurrentLoggedDate, entry);
                Console.WriteLine("You have successfully logged an entry!");
            }
            else
            {
                Console.WriteLine($"There is already a journal entry for {_currentDate:MM-dd-yyyy}:");
                Console.WriteLine(existingEntry);
                Console.Write("Do you want to edit it? (y/n): ");
                string response = Console.ReadLine().ToLower();

                if (response == "y" || response == "yes")
                {
                    Console.Write("New entry: ");
                    string newEntry = Console.ReadLine();

                    _fileHandler.ReplaceJournalEntry(_currentUser.CurrentLoggedDate, newEntry);
                    Console.WriteLine("You have successfully updated the journal entry!");
                }
                else
                {
                    Console.WriteLine("No changes made to the journal entry.");
                }
            }

            Console.ReadKey(true);
            Console.Clear();
        }


        private void ViewJournalEntry()
        {
            Console.Write("Enter the date of the journal entry you want to view (MM-dd-yyyy): ");
            string dateInput = Console.ReadLine();

            if (DateTime.TryParseExact(dateInput, "MM-dd-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime selectedDate))
            {
                string journalContent = _fileHandler.ViewJournalEntry(selectedDate);

                Console.WriteLine(journalContent);
                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                Console.Write("Invalid date format. Please use MM-dd-yyyy.");
                Console.ReadKey(true);
                Console.Clear();
            }
        }

        private void RemoveJournalEntry()
        {
            Console.WriteLine("Enter the date of the journal entry you want to remove (MM-dd-yyyy): ");
            string dateInput = Console.ReadLine();

            if (DateTime.TryParseExact(dateInput, "MM-dd-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime selectedDate))
            {
                _fileHandler.DeleteJournalEntry(selectedDate);
                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Invalid date format. Please use MM-dd-yyyy.");
            }
        }
    }
}
