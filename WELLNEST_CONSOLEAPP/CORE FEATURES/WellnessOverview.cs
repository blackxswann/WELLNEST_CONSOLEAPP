using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WELLNEST_CONSOLEAPP.FILE_HANDLING;
using WELLNEST_CONSOLEAPP.MODELS;
using Spectre.Console;

namespace WELLNEST_CONSOLEAPP.CORE_FEATURES
{
    public class WellnessOverview
    {
        private FileHandler _fileHandler;
        private UserInformation _currentUser;
        private DateTime _recentDate;
        private int _moodTotalCount, _exerciseTotalCount, _sleepTotalCount, _waterIntakeCount;
        private int _numberSkippedEntryMood, _numberSkippedEntryExercise, _numberSkippedEntrySleep, _numberSkippedEntryWaterIntake;
        private double _moodMean, _exerciseMean, _sleepMean, _waterIntakeMean;
        
        public WellnessOverview(FileHandler fileHandler, UserInformation userInformation)
        {
            _fileHandler = fileHandler;
            _currentUser = userInformation;
        }

        public void WellnessOverviewDisplay()
        {
            string[] options = new string[]
            {
                "View Weekly Wellness Report",
                "View Recommendations",
                "Go back to main menu"
            };

            int selectedIndex = 0; 

            while (true)
            {
                Console.Clear();
                DisplayOptions(options, selectedIndex, "-> WELLNESS OVERVIEW <-");  

                var key = Console.ReadKey(true).Key;
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
                                ViewWeeklyWellnessReport();
                                break;
                            case 1: 
                                Console.Clear();
                                ViewRecommendations();
                                break;
                            case 2:
                                Console.Clear();
                                return;
                        }
                        break;
                    default:
                        break;
                }
            }
        }


        private void ViewWeeklyWellnessReport()
        {
            _recentDate = _fileHandler.FindRecentDate(_currentUser);
            string dayOfWeek = _recentDate.DayOfWeek.ToString();
            DateTime[] datesFromSameWeek = new DateTime[7];
            int counter = 0;

            if (dayOfWeek == "Monday") counter = 0;
            else if (dayOfWeek == "Tuesday") counter = -1;
            else if (dayOfWeek == "Wednesday") counter = -2;
            else if (dayOfWeek == "Thursday") counter = -3;
            else if (dayOfWeek == "Friday") counter = -4;
            else if (dayOfWeek == "Saturday") counter = -5;
            else if (dayOfWeek == "Sunday") counter = -6;

            DateTime startOfWeek = _recentDate.AddDays(counter);
            for (int i = 0; i < 7; i++)
            {
                datesFromSameWeek[i] = startOfWeek.AddDays(i);
            }

            _fileHandler.GenerateWeeklyWellnessReport(_currentUser, datesFromSameWeek);
            List<Entry> weeklyEntries = _fileHandler.LoadWeeklyWellnessEntries(_currentUser, datesFromSameWeek);

            List<int> moodValues = new List<int>();
            List<int> sleepValues = new List<int>();
            List<int> waterValues = new List<int>();
            List<int> exerciseValues = new List<int>();

            int missingDataCount = 0; 
            foreach (DateTime date in datesFromSameWeek)
            {
                Entry moodEntry = FindEntryForDateAndType(weeklyEntries, date, EntryType.Mood);
                Entry sleepEntry = FindEntryForDateAndType(weeklyEntries, date, EntryType.Sleep);
                Entry waterEntry = FindEntryForDateAndType(weeklyEntries, date, EntryType.WaterIntake);
                Entry exerciseEntry = FindEntryForDateAndType(weeklyEntries, date, EntryType.Exercise);

                int moodValue = ParseEntryValue(moodEntry, EntryType.Mood);
                int sleepValue = ParseEntryValue(sleepEntry, EntryType.Sleep);
                int waterValue = ParseEntryValue(waterEntry, EntryType.WaterIntake);
                int exerciseValue = ParseEntryValue(exerciseEntry, EntryType.Exercise);

                if (moodValue == -1 || sleepValue == -1 || waterValue == -1 || exerciseValue == -1)
                {
                    missingDataCount++;
                }

                moodValues.Add(moodValue);
                sleepValues.Add(sleepValue);
                waterValues.Add(waterValue);
                exerciseValues.Add(exerciseValue);
            }

            if (missingDataCount >= 3)
            {
                Console.Clear();
                Console.WriteLine("There is not enough data to generate a meaningful weekly trend.");
                Console.WriteLine("Please input more data for the missing days to generate a full report.");
                Console.ReadKey(true);
                return;
            }

            foreach (int moodValue in moodValues)
            {
                if (moodValue != -1)
                    _moodTotalCount += moodValue;
                else
                    _numberSkippedEntryMood++;
            }
            foreach (int sleepValue in sleepValues)
            {
                if (sleepValue != -1)
                    _sleepTotalCount += sleepValue;
                else
                    _numberSkippedEntrySleep++;
            }
            foreach (int waterValue in waterValues)
            {
                if (waterValue != -1)
                    _waterIntakeCount += waterValue;
                else
                    _numberSkippedEntryWaterIntake++;
            }
            foreach (int exerciseValue in exerciseValues)
            {
                if (exerciseValue != -1)
                    _exerciseTotalCount += exerciseValue;
                else
                    _numberSkippedEntryExercise++;
            }

            _moodMean = _moodTotalCount / (7 + _numberSkippedEntryMood);
            _sleepMean = _sleepTotalCount / (7 + _numberSkippedEntrySleep);
            _waterIntakeMean = _waterIntakeCount / (7 + _numberSkippedEntryWaterIntake);
            _exerciseMean = _exerciseTotalCount / (7 + _numberSkippedEntryExercise);

            _moodMean = _numberSkippedEntryMood == 7 ? 0 : _moodMean;
            _sleepMean = _numberSkippedEntrySleep == 7 ? 0 : _sleepMean;
            _waterIntakeMean = _numberSkippedEntryWaterIntake == 7 ? 0 : _waterIntakeMean;
            _exerciseMean = _numberSkippedEntryExercise == 7 ? 0 : _exerciseMean;


            Panel panel = new Panel("                                 -> WEEKLY OVERVIEW <-                                       ");
            panel.Border = BoxBorder.Double;
            AnsiConsole.Write(panel);

            for (int i = 0; i < 7; i++)
            {
                int mood = moodValues[i];
                int sleep = sleepValues[i];
                int water = waterValues[i];
                int exercise = exerciseValues[i];

                if (mood == -1 && sleep == -1 && water == -1 && exercise == -1)
                {
                    AnsiConsole.Write(new BarChart()
                        .Width(60)
                        .AddItem($"Date:{datesFromSameWeek[i]:MM-dd-yyyy}  - No input", 0, Color.Grey));
                }
                else
                {
                    AnsiConsole.Write(new BarChart()
                        .Width(60)
                        .AddItem("            Mood", mood == -1 ? 0 : mood, mood == -1 ? Color.Grey : Color.LightYellow3)
                        .AddItem("Date:       Sleep", sleep == -1 ? 0 : sleep, sleep == -1 ? Color.Grey : Color.LightSkyBlue3)
                        .AddItem($"{datesFromSameWeek[i]:MM-dd-yyyy}  Exercise", exercise == -1 ? 0 : exercise, exercise == -1 ? Color.Grey : Color.CornflowerBlue)
                        .AddItem("            Water", water == -1 ? 0 : water, water == -1 ? Color.Grey : Color.PaleGreen1));
                }
                Console.WriteLine();
            }

            Console.WriteLine($"Mood Mean: {_moodMean:F2} | Sleep Mean: {_sleepMean:F2} | Water Intake Mean: {_waterIntakeMean:F2} | Exercise Mean: {_exerciseMean:F2}");
            Console.ReadKey(true);
        }

        private Entry FindEntryForDateAndType(List<Entry> entries, DateTime date, EntryType type)
        {
            foreach (Entry entry in entries)
            {
                if (entry.DateLogged == date && entry.EntriesType == type)
                {
                    return entry;
                }
            }
            return null; 
        }

        private int ParseEntryValue(Entry entry, EntryType type)
        {
            if (entry == null || entry.Description == "-1")
            {
                return -1; 
            }

            if (type == EntryType.Mood && Enum.TryParse(entry.Description, out MoodType mood))
            {
                return (int)mood;
            }

            if ((type == EntryType.Sleep || type == EntryType.WaterIntake || type == EntryType.Exercise) && int.TryParse(entry.Description, out int value))
            {
                return value;
            }

            return -1; 
        }


        private void ViewRecommendations()
        {
            Console.Clear();
            Console.WriteLine("\tWELLNESS RECOMMENDATIONS\n");

            Panel panel = new Panel("                              WELLNESS RECOMMENDATIONS                                  ");
            panel.Border = BoxBorder.Double;
            AnsiConsole.Write(panel);


            Panel mood = new Panel("MOOD RECOMMENDATIONS");
            mood.Border = BoxBorder.Double;
            AnsiConsole.Write(mood);
            if (_moodMean < 3)
            {
                Console.WriteLine("- Your mood average is low. Consider improving sleep, staying hydrated, and engaging in regular exercise.");
            }
            else
            {
                Console.WriteLine("- Your mood is stable. Maintain good habits to keep it that way.");
            }

            Console.WriteLine();

            Panel sleep = new Panel("SLEEP RECOMMENDATIONS");
            sleep.Border = BoxBorder.Double;
            AnsiConsole.Write(sleep);
            if (_sleepMean < 7)
            {
                Console.WriteLine("- You are getting less sleep than recommended. Aim for at least 7-8 hours of sleep per night.");
                Console.WriteLine("- Regular exercise and proper hydration can help improve sleep quality.");
            }
            else
            {
                Console.WriteLine("- Your sleep duration looks good. Keep up the good work!");
            }

            Console.WriteLine();

            Panel waterIntake = new Panel("WATER INTAKE RECOMMENDATIONS");
            waterIntake.Border = BoxBorder.Double;
            AnsiConsole.Write(waterIntake);
            if (_waterIntakeMean < 8)
            {
                Console.WriteLine("- Your water intake is below the recommended amount. Aim for at least 8 glasses of water daily.");
                if (_sleepMean < 7)
                {
                    Console.WriteLine("- Better hydration can positively affect your sleep.");
                }
                if (_exerciseMean < 30)
                {
                    Console.WriteLine("- Proper hydration is essential for better exercise performance.");
                }
            }
            else
            {
                Console.WriteLine("- Your water intake is excellent. Stay hydrated!");
            }

            Console.WriteLine();

            Panel exercise = new Panel("EXERCISE RECOMMENDATIONS");
            exercise.Border = BoxBorder.Double;
            AnsiConsole.Write(exercise);
            if (_exerciseMean < 30)
            {
                Console.WriteLine("- You are not meeting the recommended exercise duration. Aim for at least 30 minutes of activity daily.");
                if (_sleepMean < 7)
                {
                    Console.WriteLine("- Regular exercise can help improve your sleep quality.");
                }
                if (_waterIntakeMean < 8)
                {
                    Console.WriteLine("- Proper hydration can enhance exercise performance.");
                }
            }
            else
            {
                Console.WriteLine("- Your exercise routine is on track. Keep it up!");
            }

            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey(true);
            Console.Clear();
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