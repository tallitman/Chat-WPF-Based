using System;
using BusinessLayer;
using ILogger;

namespace PresentationLayer
{
    public static class Parser                                                                     // this class responsible to cope with the user input
    {
        #region properties
        private const int MESSAGES_TO_DISPLAY = 20;
        enum COMMAND_CHECK_TYPES { gID, nickname, message };                                       //types of input to check
        enum ALERTS_TYPE { attemps, message, nickname, g_ID, badChoice, menuSwitch, menu,
                               loggedIn, loggedOut, registration, loggingOut, exit };              //types of alerts 
        public static bool loggedIn = false;
        private static string loggedInNickname = "";
        private static string[] lastRegistered = { "", ""};
        #endregion
        /// <summary>
        /// this method is reponsible of taking care of the users choices and inputs through the helping methods.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="chatroom"></param>
        /// <param name="logger"></param>
        public static void parse(string str, ChatRoom chatroom, Logger logger)
        {
            switch (str)
            {
                case "register":                                                     //registration
                    {
                        if (!loggedIn)
                        {
                            string g_id = receiveCMD(COMMAND_CHECK_TYPES.gID, logger);
                            if (g_id == null) return;
                            string nickName = "";
                            bool tryReg = false;
                            while (!tryReg)
                            {
                                nickName = receiveCMD(COMMAND_CHECK_TYPES.nickname, logger);
                                if (nickName == null) return;
                                tryReg = chatroom.registration(nickName, g_id,"meanWhileRemove");
                                if (tryReg == false)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Already in use, try another name.");
                                }
                            }
                            lastRegistered[0] = g_id;
                            lastRegistered[1] = nickName;
                            Console.Clear();
                            alerts(ALERTS_TYPE.registration, logger);
                        }
                        else
                        {
                            goto default;
                        }
                    }
                    break;
                case "login":                                                        //login 
                    {
                        if (loggedIn == false)
                        {
                            string g_id = receiveCMD(COMMAND_CHECK_TYPES.gID, logger);
                            if (g_id == null) return;
                            string nickName = receiveCMD(COMMAND_CHECK_TYPES.nickname, logger);
                            if (nickName == null) return;
                            if (chatroom.login(nickName, g_id, "meanWhileRemove"))
                            {
                                loggedIn = true;
                                loggedInNickname = nickName;
                                Console.Clear();
                                alerts(ALERTS_TYPE.loggedIn, logger);

                            }
                            else
                            {
                                Console.WriteLine("There is no user matches your details, please try again.");
                            }
                        }
                        else
                        {
                            goto default;
                        }
                    }
                    break;
                case "logout":                                                       //logout
                    {
                        if (loggedIn == true)
                        {
                            chatroom.logout();
                            loggedIn = false;
                            alerts(ALERTS_TYPE.loggedOut, logger);
                        }
                        else
                            goto default;
                    }
                    break;
                case "retrieve":                                                     //Retrieve last 10 messages
                    {
                        if (loggedIn)
                        {
                            bool retrieveMsg = chatroom.retrieveMessages();
                            logger.logInfoMessage("Messages retrieve request has been sent by: " + loggedInNickname);
                            if (retrieveMsg)
                            {
                                Console.Clear();
                                Console.WriteLine("Retrieved messages succsesfully");
                            }
                            else
                            {
                                Console.WriteLine("No connection to the server.");
                            }
                        }
                        else
                        {
                            goto default;
                        }
                    }
                    break;
                case "display":                                                      //Display last retrieved messages
                    {
                        if (loggedIn)
                        {
                            logger.logInfoMessage("Display last " + MESSAGES_TO_DISPLAY + " messages request has been sent by: " + loggedInNickname);
                            string lastMsg = chatroom.displayLastMsg(MESSAGES_TO_DISPLAY);
                            Console.Clear();
                            Console.WriteLine(lastMsg);
                        }
                        else
                        {
                            goto default;
                        }
                    }
                    break;
                case "displayby":                                                    //Display all retrieved meesages send by a certain user
                    {
                        if (loggedIn)
                        {

                            logger.logInfoMessage("Display all messages request has been sent by: " + loggedInNickname);
                            string g_id = receiveCMD(COMMAND_CHECK_TYPES.gID, logger);
                            if (g_id == null) return;
                            string nickname = receiveCMD(COMMAND_CHECK_TYPES.nickname, logger);
                            if (nickname == null) return;
                            Console.Clear();
                            string displayMsgByUser = chatroom.displayAllByUser(nickname, g_id);
                            if (displayMsgByUser == null)
                            {
                                Console.WriteLine("There are no messages from " + nickname);
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine(displayMsgByUser);
                            }
                        }
                        else
                        {

                            goto default;
                        }
                    }
                    break;
                case "send":                                                         //Send a new message
                    {
                        if (loggedIn)
                        {
                            string msg = receiveCMD(COMMAND_CHECK_TYPES.message, logger);
                            if (msg == null) return;
                            logger.logInfoMessage("Send a message request has been sent by " + loggedInNickname);
                            msg = chatroom.send(msg);
                            if (msg != null)                                         //message sent: there's a connection to the server
                            {
                                chatroom.retrieveMessages();
                                logger.logInfoMessage("Retrieves message in order to implement send request by " + loggedInNickname);
                                Console.Clear();
                                Console.WriteLine(msg);

                            }
                        }
                        else
                        {
                            goto default;
                        }
                    }
                    break;
                case "exit":                                                         //exit
                    {
                        if (loggedIn)
                        {
                            alerts(ALERTS_TYPE.loggingOut, logger);
                            chatroom.logout();
                            loggedIn = false;
                        }
                        alerts(ALERTS_TYPE.exit, logger);

                    }
                    break;
                case "menu":                                                         //return to menu
                    {
                        return;
                    }
                case "help":
                    {
                        Console.WriteLine("Select one of the options by typing the appropriate word \n"
                            + "For example: if you want to log in, type 'login' \n" 
                            + "You can press 'menu' in any screen to get back to the main screen. \n");
                        return;
                    }
                default:                                                             //no command was identified
                    {
                        alerts(ALERTS_TYPE.badChoice, logger);
                        return;
                    }
            }
        }
        /// <summary>
        /// deals with inputs from user
        /// </summary>
        /// <param name="type"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static string receiveCMD(COMMAND_CHECK_TYPES type, Logger logger)
        {
            int counter = 0;                                    //attemps counter. 3 is max attempts allowed
            string str = "";
            switch (type.ToString())
            {
                case "gID":   //gID check
                    {
                        alerts(ALERTS_TYPE.menu, logger);
                        do
                        {
                            if (counter == 3) goto default;
                            alerts(ALERTS_TYPE.g_ID, logger);
                            str = Console.ReadLine();
                            if (Equals(str, "menu") || counter == 3) goto default;
                            counter++;
                        }
                        while (!(checkValidity(str, COMMAND_CHECK_TYPES.gID, logger)));  //g_ID valid numbers and range check 
                        return str;
                    }
                case "nickname":   //nickname check
                    {
                        do
                        {
                            if (counter == 3) goto default;
                            alerts(ALERTS_TYPE.nickname, logger);
                            str = Console.ReadLine();
                            if (Equals(str, "menu") || counter == 3)
                                goto default;
                            counter++;
                        }
                        while (!(checkValidity(str, COMMAND_CHECK_TYPES.nickname, logger)));       //nickname valid chars check
                        return str;
                    }
                case "message":   //message check
                    {
                        do
                        {
                            if (counter == 3) goto default;
                            alerts(ALERTS_TYPE.message, logger);
                            str = Console.ReadLine();
                            if (Equals(str, "menu") || counter == 3)
                                goto default;
                            counter++;
                        }
                        while (!(checkValidity(str, COMMAND_CHECK_TYPES.message, logger)));  //message valid chars check 
                        return str;
                    }
                default:
                    {
                        Console.Clear();
                        if (counter == 3)                   //max attempts
                            alerts(ALERTS_TYPE.attemps, logger);
                        alerts(ALERTS_TYPE.menuSwitch, logger);
                        return null;
                    }
            }
        }
        private static bool checkValidity(string str, COMMAND_CHECK_TYPES type, Logger logger)
        {
            if (str == "")
                return false;
            bool valid = true;
            str = str.ToLower();                                            //legal chars for any type of input, first is for g_id, 2nd is for nickname and 3rd is for message
            string[] validChars = {"0123456789", "0123456789abcdefghijklmnopqrstuvwxyz",
                    "0123456789abcdefghijklmnopqrstuvwxyz@^$.?,!#$%&/()=?»«@£§€{}.-;:'<@>_,]+ " };
            switch (type.ToString())
            {
                case "gID":
                    {
                        int i = 0;
                        while (valid && i < str.Length)                   // chars in validChars which are numbers only
                        {
                            string a = "" + str[i];
                            if (!(validChars[0].Contains(a)))
                                goto default;
                            i++;
                        }
                        if (Double.Parse(str) < 1 || Double.Parse(str) > 99)
                            goto default;
                        return true;
                    }
                case "nickname":
                    {
                        int i = 0;
                        while (valid && i < str.Length)                   // chars in validChars which are numbers and letters
                        {
                            string a = "" + str[i];
                            if (!(validChars[1].Contains(a)))
                                goto default;
                            i++;
                        }
                        return true;
                    }
                case "message":
                    {
                        int i = 0;
                        while (valid && i < str.Length)                   // chars in validChars which are numbers, letters and signs
                        {
                            string a = "" + str[i];
                            if (!(validChars[2].Contains(a)))
                                goto default;
                            i++;
                        }
                        return true;
                    }
                default:
                    {
                        logger.logWarnMessage("The inserted input is ilegal for: " + type.ToString() + " parameter");
                        return false;
                    }
            }
        }
        private static void alerts(ALERTS_TYPE type, Logger logger)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            switch (type.ToString())
            {
                case "attemps":          
                    {

                        logger.logWarnMessage("There were 3 bad attempts to insert an input");
                        Console.WriteLine("you have reached a max of 3 bad attempts");
                        break;
                    }
                case "message":          
                    {

                        Console.WriteLine("please enter a valid message.");
                        break;
                    }
                case "nickname":         
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("please enter a valid nickname. letters and numbers only.");
                        break;

                    }
                case "g_ID":         
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("please enter a valid group id (numbers only, valid range: 1-99)");
                        break;
                    }
                case "badChoice":        
                    {
                        logger.logWarnMessage("Something with the inserted input was wrong..");
                        Console.WriteLine("bad choice, try again");
                        break;
                    }
                case "menuSwitch":         
                    {
                        logger.logWarnMessage("User is switched back to the main menu");
                        Console.WriteLine("switching back to main menu" + '\n');
                        break;
                    }
                case "menu":      
                    {
                        Console.WriteLine("type 'menu' whenever you want to get back to main menu");
                        break;
                    }
                case "loggedIn":         
                    {
                        Console.WriteLine("Welcome back {0}", loggedInNickname);
                        break;
                    }
                case "loggedOut":         
                    {
                        Console.WriteLine("logged out successfully");
                        break;
                    }
                case "registration":       
                    {
                        logger.logInfoMessage("The user " + lastRegistered[1] + " is now registered.");
                        Console.WriteLine("Hey {0}, you have sucssesfully registerd!", lastRegistered[1]);
                        break;
                    }
                case "loggingOut":        
                    {
                        logger.logInfoMessage("The user clicked on exit. logging him out and exit");
                        Console.WriteLine("logging you out..");
                        break;
                    }
                case "exit":      
                    {
                        logger.logInfoMessage("The user went out of the CLI.");
                        Console.WriteLine("bye bye");
                        break;
                    }
            }
            Console.ResetColor();
        }
    }
}
