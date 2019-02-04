using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAccessLayer;
using ILogger;
using System.Timers;
using DataAccessLayer;
using System.Data;
using System.Data.SqlClient;
using BusinessLayer.Sort;

namespace BusinessLayer

{
    public class ChatRoom
    {
        #region PROPERTIES
        /// <summary>
        ///  server address
        /// </summary>
        private string url = "http://ise172.ise.bgu.ac.il";
        /// <summary>
        /// this attribute holds the received singelton of logger
        /// </summary>
        private Logger logger;
        /// <summary>
        /// Handlers for saving and reading from files. make a real-time backup.
        /// </summary>
        private MessageHandler msgHandler;
        private UserHandler userHandler;
        /// <summary>
        /// Current logged in user- null if there is no such at the moment.
        /// </summary>
        private User loggedinUser;
        /// <summary>
        /// lists of messages and users
        /// </summary>
        private List<Message> messages;
        private List<User> users;
        /// <summary>
        /// limit of characters per message for basic user.
        /// </summary>
        private const int MAX_MESSAGE_LENGTH = 100;
        //------------------- FOR GUI USES --------------------//
        private List<ReadOnlyMessage> messagesToPrint;

        private DateTime _lastUpadeTime = DateTime.UtcNow;
        private DateTime _dateTime = DateTime.UtcNow;
        private bool _ascending;
        #endregion

        /// <summary>
        /// constructor that loads the above attributes
        /// </summary>
        /// <param name="logger"></param>
        public ChatRoom(Logger logger)
        {
            this.logger = logger;
            loggedinUser = null;
            msgHandler = new MessageHandler(logger);
            userHandler = new UserHandler();
            messagesToPrint = new List<ReadOnlyMessage>();
            users = new List<User>();
            messages = new List<Message>();
        }

        #region REFRESH
        // ---------------------------- FOR GUI USES ---------------------------- //
        /// <summary>
        /// This method takes user's choices of sorting and filters and updates the messages screen following this.
        /// </summary>
        /// <param name="Operations">The operation to make on certain list.</param>
        public void refresh(string[] filters, bool checkTime, ISort sorter, bool ascending)
        {
            _ascending = ascending;
            if (!(filters[0] == ""))
            {
                msgHandler.addGroupFilter(filters[0]);
            }
            if (!(filters[1] == ""))
            {
                msgHandler.addNicknameFilter(filters[1]);
            }
            if (!checkTime)
                messages.Clear();
            retrieveMessages(checkTime);
            messages = sorter.DoAction(messages);
            MessageConvertor msgConvertor = new MessageConvertor();
            List<ReadOnlyMessage> newReadOnlyList = new List<ReadOnlyMessage>();
            foreach (Message msg in messages)
            {
                ReadOnlyMessage roMsg = msgConvertor.convertToReadOnly(msg);
                newReadOnlyList.Add(roMsg);
            }
            messagesToPrint = newReadOnlyList;

        }
       
        #endregion

        #region OPERATIONS

        /// <summary>
        /// checks whether entered nickname is taken or not, in case it is not taken, creates a new one
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="g_id"></param>
        /// <returns>true if proccess succeeded, false if not</returns>
        public bool registration(string nickname, string g_id, string password)
        {
            bool isRegistered = userHandler.register(nickname, g_id, password);//this register the user if not exist in user table and returns true,else false
            if (isRegistered == false)//failed to register the user
            {
                logger.logWarnMessage("the registration failed with nick=: " + nickname + " g_id:" + g_id + " password: " + password);
                return false;
            }
            //there's no such user in the system
            User newUser = new User(nickname, g_id, MAX_MESSAGE_LENGTH);
            users.Add(newUser);                                    //add user to list of users
            logger.logInfoMessage("user registered successfully");
            return true;
        }

        /// <summary>
        /// this method logs in the user with entered nickname and gid after matching check
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="g_id"></param>
        /// <returns>true if succeeded, otherwise returns false</returns>
        public bool login(string nickname, string g_id, string password)
        {
            bool isLoggedIn = userHandler.login(nickname, g_id, password);//this login the user if exist in user table and returns true,else false
            if (isLoggedIn)
            {
                User verifiedUser = new User(nickname, g_id, MAX_MESSAGE_LENGTH); //creates a logged in user
                logger.logInfoMessage(nickname + " has been created as a logged in user");
                loggedinUser = verifiedUser;      //update pointer to the loggedinUser
                return true;
            }
            logger.logWarnMessage("Trying to login with the non exist nickName: " + nickname);
            return false;
        }

        /// <summary>
        /// logs out the user
        /// </summary>
        public void logout()
        {
            logger.logInfoMessage(loggedinUser.nickName + " logged out of the system");
            loggedinUser = null;                                   
        }
 
        /// <summary>
        /// get messages from SQL
        /// </summary>
        /// <returns>true if messages retrieved</returns>
        public bool retrieveMessages(bool useTime)
        {
            try
            {
                List<IMessage> msgs;
                if (useTime)//for refresh
                {
                    msgs = msgHandler.retrieveMessages(_lastUpadeTime);
                    _lastUpadeTime = DateTime.Now;
                    _lastUpadeTime = _lastUpadeTime.ToUniversalTime();
                    updateMessages(msgs, false);
                }
                else //for apply filter 
                {
                    msgs = msgHandler.retrieveMessages();
                    if (updateMessages(msgs, true))
                        logger.logInfoMessage("messages has been successfully retrieved from server");
                    else
                        logger.logErrorMessage("Couldn't connect to SQL server..");
                }
                return true;
            }
            catch
            {
                logger.logErrorMessage("Couldn't connect to SQL server..");
                return false;
            }
        }

        /// <summary>
        /// updates the message list after messages retrieved
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="clear"></param>
        /// <returns></returns>
        private bool updateMessages(List<IMessage> msgs, bool clear)
        {
            bool ans = false;
            if (msgs == null)
                return false;
            MessageConvertor messageConvertor = new MessageConvertor();
            foreach (IMessage msg in msgs)
            {
                Message tMsg = messageConvertor.convertToMessage(msg);
                if (tMsg == null)
                    continue;
                removeMessage();
                if (clear)
                {
                    messages.Add(tMsg);
                    ans = true;
                }
                else if (!messages.Contains(tMsg))
                {
                    messages.Add(tMsg);
                    ans = true;
                }
            }
            return ans;
        }

        /// <summary>
        /// removes message if list is bigger than 200
        /// </summary>
        private void removeMessage()
        {
            if (_ascending && messages.Count > 200)
                messages.RemoveAt(0);
            else if (messages.Count > 200)
                messages.RemoveAt(messages.Count - 1);
        }

        /// <summary>
        /// adds message to the list
        /// </summary>
        /// <param name="tMsg"></param>
        private void addMessage(Message tMsg)
        {
            if (tMsg != null)
            {
                if (messages.Count >= 200)
                    messages.RemoveAt(messages.Count - 1);
                messages.Add(tMsg);//add the message to the messages list
                logger.logInfoMessage("messages has been successfully retrieved from server");
            }
        }

        /// <summary>
        /// responsible for displaying the last numToPrint messages
        /// </summary>
        /// <param name="numToPrint"> number of messages to print</param>
        /// <returns></returns>
        public string displayLastMsg(int numToPrint)
        {
            var lastMsg = (from msg in messages             //the following orders the messages by timestamp
                           orderby msg.date ascending
                           select msg).Take(numToPrint);
            string res = "";
            foreach (Message msg in lastMsg)                //convert the messages content to a returned string
            {
                res += msg + "\n";
            }
            logger.logInfoMessage("Last " + numToPrint + " messages displayed on screen");
            return res;
        }

        /// <summary>
        /// display all messages which sent by a certain user ordered by timestamp
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="g_id"></param>
        /// <returns></returns>
        public string displayAllByUser(string nickName, string g_id)
        {
            logger.logInfoMessage("Display messages request by the user " + nickName + " has been made");
            var msgByUser = (from msg in messages
                             orderby msg.date ascending
                             where msg.nickname == nickName && msg.g_ID == g_id
                             select msg);
            string res = "";
            foreach (Message msg in msgByUser)      //convert the content of each message into a returned string
                res += msg + "\n";
            if (msgByUser.Count() == 0)             //if the requested user is not in our system or the user has no messages at all
                return null;
            return res;
        }

        /// <summary>
        /// sends message from the logged in user. connects between presentation layer and User class
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public void send(string message)
        {
            try
            {
                Message msg = loggedinUser.send(message, url, DateTime.Now);  //send the message to the server address
                if (msg != null)
                {
                    removeMessage();
                    MessageConvertor msgConvertor = new MessageConvertor();
                    IMessage imsg = msgConvertor.convertToIMessage(msg);
                    // res = msgConvertor.convertToReadOnly(msg);
         
                    msgHandler.sendMessage(imsg);//save the message into the storage file
                    logger.logInfoMessage("The message were delivered successfully");
                }
            }
            catch
            { }
        }

        /// <summary>
        /// this method is called in case of editing exist message request is made by the user
        /// </summary>
        /// <param name="oldMessage">the message to edit</param>
        /// <param name="updatedBody">the updated content</param>
        public void update(ReadOnlyMessage oldMessage, string updatedBody)
        {
            try
            {
                //check its legal to edit this message
                if (!oldMessage.nickname.Equals(loggedinUser.nickName))
                {
                    logger.logWarnMessage("Failed to edit a message which does not belong to the connected user");
                }
                else
                {
                    _dateTime = DateTime.UtcNow;
                    MessageConvertor mc = new MessageConvertor();
                    Message message = mc.convertFromReadOnlyToMessage(oldMessage);
                    messages.Remove(message);
                    message.updateBody(updatedBody);
                    message.updateDate(_dateTime);
                    IMessage imsg = mc.convertToIMessage(message);
                    msgHandler.updateMessage(imsg);//save the message into the storage file
                    logger.logInfoMessage("The message were edited successfully");
                }
            }
            catch {}
        }

        /// <summary>
        /// checks the message length
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool messageCheck(string str)
        {
            if (str.Length > loggedinUser.charLimitPerMessage)
                return false;
            return true;
        }

        /// <summary>
        /// checks if the message has valid characters
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string englishCheck(string str)
        {
            bool valid = true;
            string strLower = str.ToLower();
            string allowChars = "0123456789abcdefghijklmnopqrstuvwxyz@^$.?,!#$%&/()=?»«@£§{}.-;:'*<@>_,[]+ ";
            int i = 0;
            while (valid && i < strLower.Length)                   // chars in validChars which are numbers, letters and signs
            {
                string a = "" + strLower[i];
                if (!(allowChars.Contains(a)))
                {
                    valid = false;
                }
                i++;
            }
            if (!valid)
            {
                logger.logWarnMessage("Couldn't send the received message. English and numbers only supported!");
                return "Error: cannot edit the message.\nThis language not supported!";
            }
            else
            {
                if (messageCheck(str))
                    return "The message was sent successfully";
                else
                {
                    logger.logWarnMessage("Error: cannot send the message." + '\n' + "You have reached the max characters allowed (" + loggedinUser.charLimitPerMessage + "");
                    return "Error: cannot send the message." + '\n' + "You have reached the max characters allowed (" + loggedinUser.charLimitPerMessage + ")";
                }
            }
        }
        #endregion

        #region GETTERS

        /// <summary>return the name and g.id of every user.</summary>
        public List<KeyValuePair<string, string>> getUsersList()
        {
            List<KeyValuePair<string, string>> usersList = userHandler.getUsers();
            return usersList;
        }
        
        /// <summary>return the name of logged in user.</summary>
        public string getloggedInNickName()
        {
            try
            {
                return loggedinUser.nickName;
            }
            catch
            {
                return "logged in with unknowen user";
            }
        }

        /// <summary>
        /// returns user max chars allowed
        /// </summary>
        /// <returns></returns>
        public int getUserMaxChars()
        {
            try
            {
                return loggedinUser.charLimitPerMessage;
            }
            catch
            {
                logger.logInfoMessage("return loggedinUser.charLimitPerMessage");
                return 0;
            }
        }

        /// <summary>
        /// returns messages list
        /// </summary>
        /// <returns></returns>
        public List<ReadOnlyMessage> getMessagesToPrint()
        {
            return messagesToPrint;
        }

        /// <summary>
        /// returns the message handler
        /// </summary>
        /// <returns></returns>
        public MessageHandler GetMessageHandler()
        {
            return msgHandler;
        }

        /// <summary>
        /// returns user handler
        /// </summary>
        /// <returns></returns>
        public UserHandler GetUserHandler()
        {
            return userHandler;
        }

        /// <summary>
        /// returns logged in user
        /// </summary>
        /// <returns></returns>
        public User getLoggedInUser()
        {
            return loggedinUser;
        }

        /// <summary>
        /// sets the url
        /// </summary>
        /// <param name="newUrl"></param>
        public void setUrl(string newUrl) //for tests
        {
            url = newUrl;
        }

        /// <summary>
        /// gets the url
        /// </summary>
        /// <returns></returns>
        public string getUrl()
        {
            return url;
        }

        /// <summary>
        /// gets the time
        /// </summary>
        /// <returns></returns>
        public string getTime()
        {
            return DateTime.Now.ToString("g");
        }
        #endregion
    }

}