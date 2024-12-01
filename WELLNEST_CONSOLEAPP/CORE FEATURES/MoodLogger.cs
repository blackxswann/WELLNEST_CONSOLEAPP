using Spectre.Console;
using System;
using WELLNEST_CONSOLEAPP.FILE_HANDLING;
using WELLNEST_CONSOLEAPP.MODELS;

namespace WELLNEST_CONSOLEAPP.CORE_FEATURES
{
    public class MoodLogger : EntryLogger
    {
        public MoodLogger(UserInformation currentUser, FileHandler fileHandler) : base(currentUser, fileHandler) { }

        public override void Execute()
        {
            while (true)
            {
                Panel moodHeader = new Panel("                 LOG MOOD                 ");
                moodHeader.Border = BoxBorder.Double;
                AnsiConsole.Write(moodHeader);

                string response;
                bool hasEntries = _fileHandler.CheckIfAnyEntryExists(_currentUser);

                if (!hasEntries)
                {
                    Console.WriteLine("You haven't logged any entries yet!");
                    Console.Write("Please log the date first before adding a mood entry. Press any key to return.");
                    Console.ReadKey(true);
                    Console.Clear();
                    return;
                }

                DateTime lastLoggedDate = _fileHandler.FindRecentDate(_currentUser);
                string[] tempdata = _fileHandler.LoadFileBySpecificDate(lastLoggedDate, EntryType.Mood);

                if (tempdata.Length < 3 || string.IsNullOrWhiteSpace(tempdata[2]))
                {
                    Console.WriteLine($"Current logged date: {lastLoggedDate.ToString("MM-dd-yyyy")}, Mood: No inputs yet!");
                    Console.Write("Would you like to create a mood entry? [Y/N]: ");
                    response = Console.ReadLine();

                    if (response == "Y" || response == "y")
                    {
                        LogMood(lastLoggedDate);
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
                    if (tempdata[2] == "VeryHappy")
                        Console.WriteLine($"Mood logged for {lastLoggedDate.ToString("MM-dd-yyyy")} : Very Happy");
                    else if (tempdata[2] == "VeryUnhappy")
                        Console.WriteLine($"Mood logged for {lastLoggedDate.ToString("MM-dd-yyyy")} : Very Unhappy");
                    else
                        Console.WriteLine($"Mood logged for {lastLoggedDate.ToString("MM-dd-yyyy")} : {tempdata[2]}");
                    Console.Write("Do you want to edit your mood? [Y/N]: ");
                    response = Console.ReadLine();

                    if (response == "Y" || response == "y")
                    {
                        LogMood(lastLoggedDate, true);
                        Console.Clear();
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

        private void LogMood(DateTime loggedDate, bool isEditing = false)
        {
            Console.WriteLine("[1] - Very Unhappy  [2] - Unhappy  [3] - Neutral  [4] - Happy  [5] - Very Happy");
            Console.Write("Please input your mood: ");
            string tempMoodChoice = Console.ReadLine();

            if (int.TryParse(tempMoodChoice, out int moodChoice) && Enum.IsDefined(typeof(MoodType), moodChoice))
            {
                string description = Enum.GetName(typeof(MoodType), moodChoice);

                if (isEditing)
                {
                    _fileHandler.ReplaceEntryForSpecificDate(_currentUser, EntryType.Mood, description);
                    Console.WriteLine("Your mood entry has been successfully updated!");
                }
                else
                {
                    _fileHandler.CreateAndAddEntryFileForSpecificDate(_currentUser, EntryType.Mood, description);
                    Console.WriteLine("You have successfully logged a mood entry!");
                }

                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Invalid input! Try again.");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
    }

    public enum MoodType
    {
        VeryUnhappy = 1, Unhappy, Neutral, Happy, VeryHappy
    }
}
