using Microsoft.VisualBasic.FileIO;
using Spectre.Console;
using System;
using WELLNEST_CONSOLEAPP.CORE_FEATURES;
using WELLNEST_CONSOLEAPP.FILE_HANDLING;
using WELLNEST_CONSOLEAPP.MODELS;

namespace WELLNEST_CONSOLEAPP.DISPLAY_AND_AUTHENTICATION
{
    public class DisplayAndAuthentication
    {
        private FileHandler _fileHandler;
        private List<UserInformation> _registeredUsersList;

        public DisplayAndAuthentication(FileHandler fileHandler)
        {
            _fileHandler = fileHandler;
            _registeredUsersList = _fileHandler.LoadRegisteredUsersList();
        }

        public int WelcomeMenuDisplay()
        {
            string[] options = { "LOG IN", "SIGN UP", "EXIT" };
            int selectedIndex = 0;
            ConsoleKey keyPressed;



            while (true)
            {
                Console.Clear();  

                DisplayOptions(options, selectedIndex, "WELLNEST"); 

                ConsoleKeyInfo pressedKey = Console.ReadKey(true);
                keyPressed = pressedKey.Key;

                if (keyPressed == ConsoleKey.UpArrow)
                {
                    selectedIndex--;
                    if (selectedIndex == -1)
                    {
                        selectedIndex = options.Length - 1;
                    }
                }
                else if (keyPressed == ConsoleKey.DownArrow)
                {
                    selectedIndex++;
                    if (selectedIndex == options.Length)
                    {
                        selectedIndex = 0;
                    }
                }

                if (keyPressed == ConsoleKey.Enter)
                {
                    switch (selectedIndex)
                    {
                        case 0: 
                            return 1;
                        case 1: 
                            return 2;
                        case 2: 
                            return 0;
                    }
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



        public void SignUp()
        {
            while (true)
            {
                Console.Clear();

                Panel panel = new Panel("       SIGN UP        ");
                panel.Border = BoxBorder.Double;
                AnsiConsole.Write(panel);
                Console.WriteLine();
                Console.Write("Enter full name: ");
                string tempFullName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(tempFullName))
                {
                    Console.Write("Please fill in full name!");
                    Console.ReadKey(true);
                    Console.Clear();
                    continue;
                }
                Console.Write("Enter username: ");
                string tempUsername = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(tempUsername))
                {
                    Console.WriteLine("Please fill in your username!");
                    Console.ReadKey(true);
                    Console.Clear();
                    continue;
                }
                if (DoesUsernameExist(tempUsername))
                {
                    Console.Write("Username already exists! Do you want to try again or go back to main menu? [Y/N]: ");
                    string response = Console.ReadLine();
                    if (response == "Y" || response == "y")
                    {
                        Console.Clear();
                        return;
                    }
                    else if (response == "N" || response == "n")
                    {
                        Console.Clear();
                        continue;
                    }
                    Console.Clear();
                    continue;
                }
                Console.Write("Enter password: ");
                string tempPassword = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(tempPassword))
                {
                    Console.Write("Please fill in your username!");
                    Console.ReadKey(true);
                    Console.Clear();
                    continue;
                }

                UserInformation newUser = new UserInformation(tempFullName, tempUsername, tempPassword);
                _fileHandler.UpdateRegisteredUsersList(newUser);
                _fileHandler.CreateIndividualUserInfoFile(newUser);
                _registeredUsersList = _fileHandler.LoadRegisteredUsersList();

                Console.WriteLine("You have successfully created an account!");
                Console.ReadKey(true);
                Console.Clear();
                return;
            }
        }

        public UserInformation Login()
        {
            while (true)
            {
                Console.Clear();

                Panel panel = new Panel("      LOG IN        ");
                panel.Border = BoxBorder.Double;
                AnsiConsole.Write(panel);
                Console.WriteLine();
                Console.Write("Enter username: ");
                string tempUsername = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(tempUsername))
                {
                    Console.Write("Please fill in your username!");
                    Console.ReadKey(true);
                    Console.Clear();
                    continue;
                }
                Console.Write("Enter password: ");
                string tempPassword = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(tempPassword))
                {
                    Console.Write("Please fill in your password!");
                    Console.ReadKey(true);
                    Console.Clear();
                    continue;
                }

                foreach (UserInformation currentUser in _registeredUsersList)
                {
                    if (currentUser.UserName == tempUsername && currentUser.Password == tempPassword)
                    {
                        Console.WriteLine("You have successfully logged in!");
                        Console.ReadKey(true);
                        Console.Clear();
                        return currentUser;
                    }
                }

                Console.Write("Invalid username or password! Do you want to go back to main menu? [Y/N]: ");
                string response = Console.ReadLine();

                if (response == "Y" || response == "y")
                {
                    Console.Clear();
                    return null;
                }
                else if (response == "N" || response == "n")
                {
                    Console.Clear();
                    continue;
                }
            }
        }

        private bool DoesUsernameExist(string username)
        {
            foreach (UserInformation user in _registeredUsersList)
            {
                if (user.UserName == username)
                    return true;
            }
            return false;
        }

        public void MainDashboardDisplay(UserInformation currentUser)
        {
            bool dashboardActive = true;
            int selectedIndex = 0; 

            string[] options =
            {
                "[1] - Log Entry",
                "[2] - View and Delete Entries",
                "[3] - Journal",
                "[4] - Wellness Overview",
                "[5] - Log out"
            };

            while (dashboardActive)
            {
                Console.Clear();

                string headerText = $"Welcome {currentUser.UserName}!";

                DisplayOptions(options, selectedIndex, headerText);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true); 

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : options.Length - 1; 
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex = (selectedIndex < options.Length - 1) ? selectedIndex + 1 : 0; 
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    switch (selectedIndex)
                    {
                        case 0:
                            Console.Clear();
                            LogEntryDisplay(currentUser);
                            break;
                        case 1:
                            Console.Clear();
                            EntryViewer viewAndDeleteEntry = new EntryViewer(_fileHandler, currentUser);
                            viewAndDeleteEntry.ShowEntryViewerOptionDisplay();
                            break;
                        case 2:
                            Console.Clear();
                            Journal journal = new Journal(_fileHandler, currentUser);
                            journal.JournalDisplay();
                            break;
                        case 3:
                            Console.Clear();
                            WellnessOverview wellnessOverview = new WellnessOverview(_fileHandler, currentUser);
                            wellnessOverview.WellnessOverviewDisplay();
                            break;
                        case 4:
                            Console.Clear();
                            dashboardActive = false;
                            break;
                        default:
                            Console.WriteLine("Invalid choice! Try again.");
                            break;
                    }
                }
            }
        }


        public void LogEntryDisplay(UserInformation currentUser)
        {
            bool dashboardActive = true;
            string[] options = new string[]
            {
                "Log Date",
                "Log Mood",
                "Log Sleep",
                "Log Water Intake",
                "Log Exercise",
                "Go back to main dashboard"
            };

            int selectedIndex = 0;

            while (dashboardActive)
            {
                Console.Clear();
                DisplayOptions(options, selectedIndex, "-> LOG ENTRY <-");  

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
                                EntryLogger logDate = new DateLogger(currentUser, _fileHandler);
                                logDate.Execute();
                                break;
                            case 1:
                                Console.Clear();
                                EntryLogger logMood = new MoodLogger(currentUser, _fileHandler);
                                logMood.Execute();
                                break;
                            case 2:
                                Console.Clear();
                                EntryLogger logSleep = new SleepLogger(currentUser, _fileHandler);
                                logSleep.Execute();
                                break;
                            case 3:
                                Console.Clear();
                                EntryLogger logWaterIntake = new WaterIntakeLogger(currentUser, _fileHandler);
                                logWaterIntake.Execute();
                                break;
                            case 4:
                                Console.Clear();
                                EntryLogger logExercise = new ExerciseLogger(currentUser, _fileHandler);
                                logExercise.Execute();
                                break;
                            case 5:
                                return;  
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
