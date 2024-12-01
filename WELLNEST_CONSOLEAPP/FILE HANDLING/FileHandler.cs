using System;
using System.Collections.Generic;
using System.IO;
using WELLNEST_CONSOLEAPP.MODELS;

namespace WELLNEST_CONSOLEAPP.FILE_HANDLING
{
    public class FileHandler
    {
        private List<UserInformation> _registeredUsersList;
        private string _baseFolderPath = @"C:\\Users\\Shayne Rose\\Desktop\\PUERTO - WELLNEST CONSOLE APPLICATION\\UserData";
        private string _registeredUserPath;
        private string _individualUserFolderPath;

        public FileHandler()
        {
            _registeredUsersList = new List<UserInformation>();
            DoesBaseFolderExist();
            DoesRegisteredUsersListExist();
            LoadRegisteredUsersList();
        }

        private void DoesBaseFolderExist()
        {
            try
            {
                if (!Directory.Exists(_baseFolderPath))
                {
                    Directory.CreateDirectory(_baseFolderPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the main folder: {ex.Message}");
            }
        }

        private void DoesRegisteredUsersListExist()
        {
            _registeredUserPath = Path.Combine(_baseFolderPath, "RegisteredUsersList.csv");

            if (!File.Exists(_registeredUserPath))
            {
                try
                {
                    using (StreamWriter writer = File.CreateText(_registeredUserPath))
                    {
                        writer.WriteLine("FULL NAME:,USERNAME:,PASSWORD:");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while creating the registered users list file: {ex.Message}");
                }
            }
        }

        private void DoesIndividualUserFolderExist(string username)
        {
            _individualUserFolderPath = Path.Combine(_baseFolderPath, username);

            if (!Directory.Exists(_individualUserFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(_individualUserFolderPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while creating the user folder: {ex.Message}");
                }
            }
        }

        public void UpdateRegisteredUsersList(UserInformation user)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(_registeredUserPath))
                {
                    writer.WriteLine($"{user.FullName},{user.UserName},{user.Password}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the list of registered users: {ex.Message}");
            }
        }

        public List<UserInformation> LoadRegisteredUsersList()
        {
            try
            {
                using (StreamReader reader = File.OpenText(_registeredUserPath))
                {
                    reader.ReadLine(); 

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] tempData = line.Split(',');

                        if (tempData.Length == 3)
                        {
                            UserInformation user = new UserInformation(tempData[0], tempData[1], tempData[2]);
                            _registeredUsersList.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading registered users: {ex.Message}");
            }

            return _registeredUsersList;
        }

        public void CreateIndividualUserInfoFile(UserInformation user)
        {
            DoesIndividualUserFolderExist(user.UserName);
            string userInfoFilePath = Path.Combine(_individualUserFolderPath, $"{user.UserName}info.csv");

            if (!File.Exists(userInfoFilePath))
            {
                try
                {
                    using (StreamWriter writer = File.CreateText(userInfoFilePath))
                    {
                        writer.WriteLine($"FULL NAME:,USERNAME:,PASSWORD:");
                        writer.WriteLine($"{user.FullName},{user.UserName},{user.Password}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while creating individual user info file: {ex.Message}");
                }
            }
        }

        public void CreateAndAddEntryFileForSpecificDate(UserInformation user, EntryType entryType, string description)
        {
            string currentDate = user.CurrentLoggedDate.ToString("MM-dd-yyyy");

            DoesIndividualUserFolderExist(user.UserName);
            string individualUserEntryPath = Path.Combine(_individualUserFolderPath, $"{currentDate}.csv");

            if (!File.Exists(individualUserEntryPath))
            {
                try
                {
                    using (StreamWriter writer = File.CreateText(individualUserEntryPath))
                    {
                        writer.WriteLine($"DATE LOGGED:,ENTRY TYPE:,DESCRIPTION:");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while creating an entry file: {ex.Message}");
                }
            }

            try
            {
                using (StreamWriter writer = File.AppendText(individualUserEntryPath))
                {
                    writer.WriteLine($"{user.CurrentLoggedDate.Date},{entryType},{description}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding an entry: {ex.Message}");
            }
        }

        public void ReplaceEntryForSpecificDate(UserInformation user, EntryType entryType, string description)
        {
            string currentDate = user.CurrentLoggedDate.ToString("MM-dd-yyyy");
            DoesIndividualUserFolderExist(user.UserName);
            string individualUserEntryPath = Path.Combine(_individualUserFolderPath, $"{currentDate}.csv");

            if (!File.Exists(individualUserEntryPath))
            {
                Console.WriteLine("The specified file does not exist. No replacement can be made.");
                return;
            }

            List<string> updatedLines = new List<string>();

            try
            {
                using (StreamReader reader = File.OpenText(individualUserEntryPath))
                {
                    string header = reader.ReadLine(); 
                    if (!string.IsNullOrEmpty(header))
                    {
                        updatedLines.Add(header); 
                    }

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] lineData = line.Split(',');
                        if (lineData.Length >= 2 && Enum.TryParse(lineData[1], out EntryType existingType) && existingType == entryType)
                        {
                            updatedLines.Add($"{user.CurrentLoggedDate.Date},{entryType},{description}");
                        }
                        else
                        {
                            updatedLines.Add(line);
                        }
                    }
                }

                using (StreamWriter writer = File.CreateText(individualUserEntryPath))
                {
                    foreach (string line in updatedLines)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while replacing an entry: {ex.Message}");
            }
        }

        public bool CheckIfAnyEntryExists(UserInformation user)
        {
            DoesIndividualUserFolderExist(user.UserName);

            string[] dateFiles = Directory.GetFiles(_individualUserFolderPath, "*.csv");

            foreach (string file in dateFiles)
            {
                if (Path.GetFileName(file) != $"{user.UserName}info.csv")
                {
                    return true;
                }
            }

            return false;
        }

        public DateTime FindRecentDate(UserInformation user)
        {
            DoesIndividualUserFolderExist(user.UserName);
            DateTime mostRecentDate = DateTime.MinValue;

            try
            {
                string[] dateFiles = Directory.GetFiles(_individualUserFolderPath, "*.csv");

                foreach (string date in dateFiles)
                {
                    string dateHolder = Path.GetFileNameWithoutExtension(date);

                    if (DateTime.TryParse(dateHolder, out DateTime fileDate))
                    {
                        if (fileDate > mostRecentDate)
                        {
                            mostRecentDate = fileDate;
                        }
                    }
                }

                user.CurrentLoggedDate = mostRecentDate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while finding the recent date: {ex.Message}");
            }

            return mostRecentDate;
        }

        public string[] LoadFileBySpecificDate(DateTime recentDate, EntryType entryType)
        {
            string individualUserEntryPath = Path.Combine(_individualUserFolderPath, $"{recentDate:MM-dd-yyyy}.csv");
            string[] data = new string[3]; 

            try
            {
                using (StreamReader reader = File.OpenText(individualUserEntryPath))
                {
                    reader.ReadLine(); 
                    reader.ReadLine(); 

                    string lines;
                    while ((lines = reader.ReadLine()) != null)
                    {
                        string[] tempData = lines.Split(',');

                        if (tempData[1].Equals(entryType.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            data[0] = tempData[0];
                            data[1] = tempData[1];
                            data[2] = tempData[2];

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading an entry for specific date: {ex.Message}");
            }

            return data;
        }

        public List<string> ViewAndDeleteFileBySpecificDate(DateTime recentDate, int choice, UserInformation user)
        {
            List<string> data = new List<string>();
            DoesIndividualUserFolderExist(user.UserName);
            string individualUserEntryPath = Path.Combine(_individualUserFolderPath, $"{recentDate:MM-dd-yyyy}.csv");

            if (!File.Exists(individualUserEntryPath))
            {
                Console.WriteLine("No file exists for the specified date.");
                return null; 
            }

            try
            {
                using (StreamReader reader = File.OpenText(individualUserEntryPath))
                {
                    reader.ReadLine(); 
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        data.Add(line);
                    }
                }

                if (choice == 1)
                {
                    File.Delete(individualUserEntryPath);
                    Console.WriteLine("The file has been deleted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while viewing or deleting the file: {ex.Message}");
            }

            return data;
        }

        public void SaveJournalEntry(DateTime entryDate, string journalContent)
        {
            string journalFilePath = Path.Combine(_individualUserFolderPath, $"{entryDate:MM-dd-yyyy}-journal.csv");

            try
            {
                if (!File.Exists(journalFilePath))
                {
                    using (StreamWriter writer = File.CreateText(journalFilePath))
                    {
                        writer.WriteLine("DATE,CONTENT"); 
                    }
                }

          
                using (StreamWriter writer = File.AppendText(journalFilePath))
                {
                    writer.WriteLine($"{entryDate:MM-dd-yyyy},{journalContent}"); 
                }

                Console.WriteLine($"Journal entry for {entryDate:MM-dd-yyyy} saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving journal entry: {ex.Message}");
            }
        }

        public string ViewJournalEntry(DateTime entryDate)
        {
            string journalFilePath = Path.Combine(_individualUserFolderPath, $"{entryDate:MM-dd-yyyy}-journal.csv");

            if (!File.Exists(journalFilePath))
            {
                return $"No journal entry exists for {entryDate:MM-dd-yyyy}.";
            }

            try
            {
                using (StreamReader reader = File.OpenText(journalFilePath))
                {
                    reader.ReadLine(); 
                    string contentLine = reader.ReadLine();

                    if (!string.IsNullOrEmpty(contentLine))
                    {
                        string[] entryData = contentLine.Split(',');

                        if (entryData.Length == 2)
                        {
                            return $"Journal Entry for {entryData[0]}: {entryData[1]}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error reading journal entry: {ex.Message}";
            }

            return $"Journal entry file exists but content is invalid for {entryDate:MM-dd-yyyy}.";
        }
        public void ReplaceJournalEntry(DateTime entryDate, string newEntry)
        {
            string journalFilePath = Path.Combine(_individualUserFolderPath, $"{entryDate:MM-dd-yyyy}-journal.csv");

            if (!File.Exists(journalFilePath))
            {
                Console.WriteLine("No journal entry found to replace.");
                return;
            }

            try
            {
                List<string> updatedLines = new List<string>();

                using (StreamReader reader = File.OpenText(journalFilePath))
                {
                    string header = reader.ReadLine(); 
                    updatedLines.Add(header);

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] entryData = line.Split(',');

                        if (entryData.Length == 2)
                        {
                            updatedLines.Add($"{entryData[0]},{newEntry}");
                        }
                        else
                        {
                            updatedLines.Add(line);
                        }
                    }
                }

                File.WriteAllLines(journalFilePath, updatedLines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error replacing journal entry: {ex.Message}");
            }
        }

        public void DeleteJournalEntry(DateTime entryDate)
        {
            string journalFilePath = Path.Combine(_individualUserFolderPath, $"{entryDate:MM-dd-yyyy}-journal.csv");

            if (!File.Exists(journalFilePath))
            {
                Console.WriteLine($"No journal entry exists for {entryDate:MM-dd-yyyy} to delete.");
                return;
            }

            try
            {
                File.Delete(journalFilePath);
                Console.WriteLine($"Journal entry for {entryDate:MM-dd-yyyy} deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting journal entry: {ex.Message}");
            }
        }


        public void GenerateWeeklyWellnessReport(UserInformation currentUser, DateTime[] datesFromSameWeek)
        {
            string weeklyReportPath = Path.Combine(_individualUserFolderPath, "WeeklyReports");
            Directory.CreateDirectory(weeklyReportPath);

            string fileName = $"WeeklyReport_{datesFromSameWeek[0]:MM-dd-yyyy}_to_{datesFromSameWeek[6]:MM-dd-yyyy}.csv";
            string reportFilePath = Path.Combine(weeklyReportPath, fileName);

            List<string> reportLines = new List<string>();
            reportLines.Add("DATE,ENTRY TYPE,DESCRIPTION"); 

            EntryType[] expectedEntryTypes = new EntryType[] { EntryType.Date, EntryType.Mood, EntryType.WaterIntake, EntryType.Exercise, EntryType.Sleep };

            for (int i = 0; i < 7; i++)
            {
                DateTime date = datesFromSameWeek[i];
                string entryFilePath = Path.Combine(_individualUserFolderPath, $"{date:MM-dd-yyyy}.csv");

                string[] dailyEntries = new string[expectedEntryTypes.Length];

                for (int j = 0; j < dailyEntries.Length; j++)
                {
                    dailyEntries[j] = "-1";
                }

                if (File.Exists(entryFilePath))
                {
                    try
                    {
                        using (StreamReader reader = File.OpenText(entryFilePath))
                        {
                            string header = reader.ReadLine(); 
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                string[] entryData = line.Split(',');

                                if (entryData.Length >= 3)
                                {
                                    EntryType entryType;
                                    if (Enum.TryParse(entryData[1], out entryType)) 
                                    {
                                        int index = Array.IndexOf(expectedEntryTypes, entryType);
                                        if (index != -1)
                                        {
                                            dailyEntries[index] = entryData[2]; 
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading entries for {date:MM-dd-yyyy}: {ex.Message}");
                    }
                }

                for (int j = 0; j < expectedEntryTypes.Length; j++)
                {
                    reportLines.Add($"{date:MM-dd-yyyy},{expectedEntryTypes[j]},{dailyEntries[j]}");
                }
            }
            try
            {
                using (StreamWriter writer = File.CreateText(reportFilePath))
                {
                    foreach (string line in reportLines)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing weekly wellness report: {ex.Message}");
            }
        }


        public List<Entry> LoadWeeklyWellnessEntries(UserInformation currentUser, DateTime[] datesFromSameWeek)
        {
            List<Entry> entries = new List<Entry>();

            for (int i = 0; i < 7; i++)
            {
                DateTime date = datesFromSameWeek[i];
                string entryFilePath = Path.Combine(_individualUserFolderPath, $"{date:MM-dd-yyyy}.csv");

                if (File.Exists(entryFilePath))
                {
                    try
                    {
                        using (StreamReader reader = File.OpenText(entryFilePath))
                        {
                            string header = reader.ReadLine(); 
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                string[] entryData = line.Split(',');

                                if (entryData.Length >= 3)
                                {
                                    EntryType entryType;
                                    if (Enum.TryParse(entryData[1], out entryType)) 
                                    {
                                        Entry entry = new Entry(
                                            date,
                                            entryType,
                                            entryData[2] 
                                        );
                                        entries.Add(entry);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Invalid entry type in file for {date:yyyy-MM-dd}");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading entries for {date:yyyy-MM-dd}: {ex.Message}");
                    }
                }
                else
                {
                    entries.Add(new Entry(date, EntryType.Date, "No entries for this day"));
                }
            }

            return entries;
        }

        

    }
}
