using System;
using BusinessLayer;
using ILogger;
using System.Text;

namespace PresentationLayer
{
    public class Menu                                       //responsible for the menu display
    {
        private static string loggedInMenu;                 //menu string
        private static string loggedOutMenu;
        /// <summary>
        /// program starts from here
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)              
        {
            Logger logger = Logger.Instance;                // initiates new logger and declares that startup was successful
            logger.logInfoMessage("The program was successfully started ");
            string input = "";
            generateMenu();                                 //generate types of menus
            ChatRoom chatroom = new ChatRoom(logger);
            Parser.parse("help", chatroom, logger);         //display help to instruct user
            do
            {                                               //menu print and get inputs from user
                printMenu();
                input = Console.ReadLine().ToLower();       // the main screen of the program
                Parser.parse(input, chatroom, logger);      // Parse and handle the input using the BusinessLayer ChatRoom
            } while (!input.Equals("exit"));
            logger.logInfoMessage("Session ended- exit from CLI");
        }
        /// <summary>
        /// generates a menu regarding the status of the user: logged in or logged out 
        /// </summary>
        private static void generateMenu()
        {
            string[] loggedOutOptions = { "Register", "Login", "Help", "Exit" };        //menu options
            string[] loggedOutTypes = { "register", "login", "help", "exit" };          //input types
            string[] loggedInOptions = {"Logout", "Retrieve last 10 messages",
                "Display last 20 retrieved messages", "Display retrieved meesages by a certain user",
                "Send a message", "Help", "Exit"};
            string[] loggedInTypes = { "logout", "retrieve", "display", "displayby", "send", "help", "exit" };
            string format = "{0,-5} {1,-15} {2,-15}" + Environment.NewLine;             //menu spaces 
            Console.ResetColor();
            var loggedInMenuBuilder = new StringBuilder().AppendFormat(format, "", "type", "to ");
            for (int i = 0; i < loggedInOptions.Length; i++)                            //inserts menu onto string
                loggedInMenuBuilder.AppendFormat(format, "", loggedInTypes[i], loggedInOptions[i]);
            var loggedOutMenuBuilder = new StringBuilder().AppendFormat(format, "", "type", "to ");
            for (int i = 0; i < loggedOutOptions.Length; i++)
                loggedOutMenuBuilder.AppendFormat(format, "", loggedOutTypes[i], loggedOutOptions[i]);
            loggedOutMenu = loggedOutMenuBuilder.ToString();
            loggedInMenu = loggedInMenuBuilder.ToString();
        }
        /// <summary>
        /// prints menu
        /// </summary>
        private static void printMenu()
        {
            Console.WriteLine("What would you like to do?\n");
            if (Parser.loggedIn)
                Console.WriteLine(loggedInMenu);
            else
                Console.WriteLine(loggedOutMenu);
        }
    }
}