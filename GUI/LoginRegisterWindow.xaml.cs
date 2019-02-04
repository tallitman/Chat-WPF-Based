using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ILogger;
using BusinessLayer;

namespace GUI
{
    /// <summary>
    /// Interaction logic for LoginRegisterWindow.xaml
    /// </summary>
    public partial class LoginRegisterWindow : Window
    {
        private string _securedPassword;//holds the hashed password
        private bool _passwordIsValid;//flag that tells if inserted password is valid
        
        enum EnterType : int { Login = 1, Registration }; //appearance type for the window
        enum sendDetails : int { Login = 1, Register };  //appearnce type for button
        private int _enterTypeStatus;//the type of window received from main window

        public int enterTypeStatus
        {
            get
            {
                return _enterTypeStatus;
            }
            set
            {
                this._enterTypeStatus = value;
            }
        }
        public Logger logger;
        public ChatRoom chatroom;

        private static string loggedInNickname = "";//logged in nickname
        private static string[] lastRegistered = { "", "" };//holds the group id and nickname of the last registered user

        private ObrModels _main = new ObrModels();//instance of observable model 


        public LoginRegisterWindow(int gotEnterType, Logger logger, ChatRoom chatroom)
        {
            InitializeComponent();
            this.DataContext = _main;
            comboBox_g_id_Init();

            enterTypeStatus = gotEnterType;  //set login/register
            this.logger = logger;
            this.chatroom = chatroom;
            //determines the name of the opened window login or registration
            _main.logRegStatus = "" + (EnterType)enterTypeStatus;
            //determines the appearance of the send button
            _main.logRegBtnStatus = "" + (sendDetails)gotEnterType;
            logger.logInfoMessage("Moved to " + (EnterType)gotEnterType + " window");
        }
        /// <summary>
        /// handles the changes in the username texbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void user_name_textBoxHasChanged(object sender, RoutedEventArgs eventArgs)
        {
            _main.user_name = "";
        }
        /// <summary>
        /// this event handles the changes in the password passwordbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void pswrd_hasChanged(object sender, RoutedEventArgs eventArgs)
        {
            PasswordBox pb = sender as PasswordBox;
            _passwordIsValid = checkValidityPass(pb.Password, logger);//update flag if the inserted for password is valid
            _securedPassword = Hashing.getHashedPassWithSalt(pb.Password); 
            
        }
        /// <summary>
        /// handles the login or registeration proccess
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void loginRegisterProcess(object sender, RoutedEventArgs eventArgs)
        {
            

            string g_id = _main.g_id_selected; //get the group id
            string nickName = _main.user_name; //user name nickname
            bool validPassword = _passwordIsValid; //get the checked password status
            bool validUserName = checkValidity(nickName, logger); //check validity of inserted username

            if (!((validUserName) && (validPassword)))//check validity of input both inserted g_id and Username
            {
                MessageBox.Show("user name or password are illegal:\nUsername and password characters allowed are: letters and numbers\npassword length should include at least 4 characters and less than 17");
                if (!validUserName)
                {
                    logger.logWarnMessage("The inserted input for UserName or password is illegal");
                }
            }
            else //handle the case of valid inputs from the user. continue process
            {
                
                string hashedPassword = _securedPassword; //get the hashed secured password
                switch (this._enterTypeStatus)
                {
                    case 1://login
                        {
                            if (chatroom.login(nickName, g_id, hashedPassword))//logged in successfully
                            {
                                loggedInNickname = nickName;
                                //moving to chatroom window
                                logger.logInfoMessage("successful login: moving to Chatroom Window");
                                ChatRoomWindow crWin = new ChatRoomWindow(chatroom, logger);//open chatroom window
                                crWin.Show();
                                this.Close();
                            }
                            else //log in failed
                            {
                                MessageBox.Show("There is no user matches your details, please try again.");
                            }
                        }
                        break;
                    case 2://register
                        {
                            bool tryReg = chatroom.registration(nickName, g_id, hashedPassword);
                            if (tryReg == false)//registration failed
                            {
                                MessageBox.Show("User Name is already in use in the required Group, try again.");
                            }
                            else//successfully registration
                            {
                                lastRegistered[0] = g_id;
                                lastRegistered[1] = nickName;
                                MessageBox.Show("Hi " + lastRegistered[1] + ", You have successfully registered");
                                goToMainWindow();
                            }
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// checks whether the inserted information is correct
        /// </summary>
        /// <param name="str"> a string which includes a user's nickname</param>
        /// <param name="type">an integer which specifies which information is entered: gid or nickname </param>
        /// <param name="logger">keeps the errors onto file</param>
        /// <returns></returns>
        private static bool checkValidity(string str, Logger logger)
        {
            if (str == "")
                return false;
            bool valid = true;
            str = str.ToLower();                              //legal chars for any type of input, first is for g_id, 2nd is for nickname
            string validChars = "0123456789abcdefghijklmnopqrstuvwxyz";
            int i = 0;
            while (valid && i < str.Length)                   //chars in validChars which are numbers and letters
            {
                string a = "" + str[i];
                if (!(validChars.Contains(a)))
                {
                    logger.logWarnMessage("The inserted input is ilegal for nickname");
                    return false;
                }
                i++;
            }
            return true;
        }
        /// <summary>
        /// This method responsible for moving to main window
        /// </summary>
        private void goToMainWindow()
        {
            MainWindow mainWin = new MainWindow();
            mainWin.Show();
            this.Close();
        }
        //log in /register button event
        private void button_logReg_Click(object sender, RoutedEventArgs e)
        {
            //call for login register process
            loginRegisterProcess(sender, e);
        }
        //back button event
        private void button_Back_Click(object sender, RoutedEventArgs e)
        {
            goToMainWindow();
        }
        //initial the combobox of the group id's
        private void comboBox_g_id_Init()
        {
            _main.setGidItemsInComboBox("Choose your Group ID");
            for (int i = 1; i <= 40; i++)
            {
                _main.setGidItemsInComboBox(i.ToString());

            }
        }


        /// <summary>
        /// this method checks the validity of inserted input by english characters and numbers only without spaces
        /// check the length of the password as well
        /// </summary>
        /// <param name="str"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private bool checkValidityPass(string str, Logger logger)
        {
            bool valid = true;
            string strLower = str.ToLower();
            string allowChars = "0123456789abcdefghijklmnopqrstuvwxyz";
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
            //check the length of the password
            if (str.Length < 4 || str.Length > 16)
            {
                valid = false;
            }
            if (valid == false)
            {
                logger.logWarnMessage("illegal password- not only english chars and numbers (without spaces)");
            }
            return valid;
        }
    }
}
