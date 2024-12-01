using System.Linq.Expressions;
using WELLNEST_CONSOLEAPP.DISPLAY_AND_AUTHENTICATION;
using WELLNEST_CONSOLEAPP.FILE_HANDLING;
using WELLNEST_CONSOLEAPP.MODELS;

namespace WELLNEST_CONSOLEAPP
{
    internal class Program
    { 
        static void Main(string[] args)
        {

            Program p = new Program();

            bool sessionActive = true;
            int choice;
            UserInformation currentUser; 

            FileHandler fileHandler = new FileHandler();
            DisplayAndAuthentication displayAndAuthentication = new DisplayAndAuthentication(fileHandler);

            while (sessionActive)
            {
                choice = displayAndAuthentication.WelcomeMenuDisplay(); 
                
                switch (choice)
                {
                    case 1:
                        {
                            Console.Clear();
                            currentUser = displayAndAuthentication.Login();
                            if (currentUser == null)
                            {
                                Console.Clear();
                                continue; 
                            }
                            displayAndAuthentication.MainDashboardDisplay(currentUser);
                            break; 
                        }
                    case 2:
                        {
                            Console.Clear();
                            displayAndAuthentication.SignUp();
                            break;
                        }
                    case 3:
                        {
                            Console.WriteLine("Bye!");
                            sessionActive = false; 
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid input, try again!");
                            Console.ReadKey(true);
                            Console.Clear();
                            break;
                        }
                }
            }
            
        }
    }
}
