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
using BusinessLayer;
using ILogger;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using BusinessLayer.Sort;

namespace GUI
{
    /// <summary>
    /// Interaction logic for ChatRoomWindow.xaml
    /// </summary>
    public partial class ChatRoomWindow : Window
    {
        #region PROPERTIES
        private MessageItem _pointedMessage;
        private bool _isEdit;                                       //tells if current message for update or sending-true=update
        private ChatRoom chatroom;                                  //chat room logics
        private ISort sortby;                                       //object which holds user's sort by choice 
        private Logger logger;
        private string[] filters = { "", "" };                      // 0 is gid, 1 is nickname
        private ObrModels _main = new ObrModels();                  //observer instance to handle bindings
        private string loggedInNickname = "";
        private bool clearChatroom = true;                          //if user wants to sort or filter, clear the chat
        DispatcherTimer dispatcherTimer = new DispatcherTimer();    //timer handle refreshes 
        #endregion

        #region INITIAL
        /// <summary>
        /// chatroom window constructor
        /// </summary>
        /// <param name="chatroom">chatroom logics handler</param>
        /// <param name="logger">keeps users chosings and error onto file</param>

        public ChatRoomWindow(ChatRoom chatroom, Logger logger)
        {
            InitializeComponent();
            this.DataContext = _main;
            this.logger = logger;
            this.chatroom = chatroom;
            sortby = new SortbyTimestamp(_main.Ascending);
            loggedInNickname = chatroom.getloggedInNickName();
            _main.LoggedInContent = loggedInNickname;
            dispatcherTimer.Tick += dispatcherTimer_Tick;           // initialize in constructor
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 2);    // initialize in constructor
            dispatcherTimer.Start();                                // initialize in constructor
            chatRefresh(false);
        }

        /// <summary>
        /// each two seconds period, this method is called to refresh the messages screen using refresh function
        /// </summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            chatRefresh(true);
        }
      
        /// <summary>
        /// scroll down automaticly
        /// </summary>
        private void listBoxLoad(object sender, RoutedEventArgs e)
        {
            var listBox = (ListBox)sender;
            listBox.Items.MoveCurrentToLast();
            listBox.ScrollIntoView(listBox.Items.CurrentItem);
        }
        #endregion

        #region REFRESH
        /// <summary>
        /// refreshes the chatroom messages according to user's sorting and filtering choices. 
        /// as default shows ascending timestamp order
        /// adds messages if there are new
        /// clears and adds messages whenever user wants to sort or filter
        /// </summary>
        private void chatRefresh(bool checkTime)
        {
            chatroom.refresh(filters, checkTime, sortby, _main.Ascending);
            _main.ServerTime = chatroom.getTime();
            List<ReadOnlyMessage> messagesToPrint = chatroom.getMessagesToPrint();
            ObservableCollection<MessageItem> msgPrint = new ObservableCollection<MessageItem>();
            foreach (ReadOnlyMessage msg in messagesToPrint)     //if no selection made on sort or filter, 
            {
                MessageItem tempItem = new MessageItem(msg, msg.nickname.Equals(loggedInNickname));
                if (_pointedMessage != null && tempItem.Equals(_pointedMessage))
                {
                    msgPrint.Add(_pointedMessage);
                    continue;
                }
                msgPrint.Add(tempItem);
            }

            if (clearChatroom && msgPrint.Count != 0)
            {
                _main.Messages = msgPrint;
                clearChatroom = false;
                _main.scroll();
            }
            else if (listChanged(msgPrint))
                _main.Messages = msgPrint;
            List<KeyValuePair<string, string>> usersList = chatroom.getUsersList();
            _main.setItemsNickname(usersList);
   
        }

        /// <summary>
        /// determines whether the list was changed or not
        /// </summary>
        /// <param name="msgPrint"></param>
        /// <returns></returns>
        private bool listChanged(ObservableCollection<MessageItem> msgPrint)
        {
            if( _main.Messages == null || _main.Messages.Count == 0)
                return true;
            if (msgPrint.Count == 0) return true;
            foreach (var message in msgPrint)
                if (!_main.Messages.Contains(message))
                    return true;
            return false;
            //return !msgPrint.Last().Equals(_main.Messages.Last());
        }
        #endregion

        /// <summary>
        /// apply changes button event handler
        /// sorts and filters as user wishes and finally updates chatroom using chatRefresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FILTER_SORT_Click(object sender, RoutedEventArgs e)
        {
            _main.Indicator = "";
            string filterBy = _main.FilterByCombobox;
            string filterGID = _main.FilterByGid;
            string filterNickname = _main.FilterByNickname;
            switch (filterBy)
            {
                case ("None"):
                    {
                        filters[0] = "";
                        filters[1] = "";
                    }
                    break;
                case ("Group"):
                    if (filterGID != null)
                    {
                        filters[0] = filterGID;
                        filters[1] = "";
                    }
                    break;
                case ("User"):
                    if (filterNickname != null)
                    {
                        if (filterNickname.Equals(""))
                        {
                            indicatorColor("red");
                            _main.Indicator = "Choose nickname first";
                        }
                        else
                        {
                            filters[0] = filterGID;
                            filters[1] = filterNickname;
                        }
                    }
                    break;
                default: break;
            }
            string sortBy = _main.SortByCombobox;
            switch (sortBy)
            {
                case ("Timestamp"):
                    sortby = new SortbyTimestamp(_main.Ascending);
                    break;
                case ("Nickname"):
                    sortby = new SortbyNickname(_main.Ascending);
                    break;
                case ("Group, Nickname and Timestamp"):
                    sortby = new SortbyAll(_main.Ascending);
                    break;
            }
            clearChatroom = true;
            chatRefresh(false);
        }

        #region SEND BUTTON
        /// <summary>
        /// send button event handler
        /// sends the user's message to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Send_Click(object sender, RoutedEventArgs e)
        {
            if (_main.MessageContent.Trim().Equals(""))
            {
                indicatorColor("red");
                _main.Indicator = "Cannot send an empty message";
                logger.logInfoMessage("Send a empty message request has been sent by " + loggedInNickname);
            }
            else
            {
                if (_isEdit)
                    edit();
                else
                    send();
                clearChatroom = true;
                chatRefresh(true);
            }
            _main.MessageContent = "";
        }

        /// <summary>
        /// edits the message
        /// </summary>
        private void edit()
        {
            logger.logInfoMessage("Edit a message request has been sent by " + loggedInNickname);
            string updateReport = chatroom.englishCheck(_main.MessageContent);
            if (updateReport.Equals("The message was sent successfully"))
            {
                updateReport = "The message was editted successfully";
                chatroom.update(_pointedMessage.message, _main.MessageContent);
                _main.IsTextBoxFocused = false;
                indicatorColor("black");
                _isEdit = false;
            }
            else
                indicatorColor("red");
            _main.Indicator = updateReport;
        }
        /// <summary>
        /// sends the message
        /// </summary>
        private void send()
        {
            logger.logInfoMessage("Send a message request has been sent by " + loggedInNickname);
            string sendReport = chatroom.englishCheck(_main.MessageContent);
            if (sendReport.Equals("The message was sent successfully"))
            {
                chatroom.send(_main.MessageContent);
                indicatorColor("black");
            }
            else
                indicatorColor("red");
            _main.Indicator = sendReport;
        }

        /// <summary>
        /// when the user types in new message textbox, this method updates the textbox content binding property 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_NewMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            string msg = textBox.Text;
            if (msg.Equals("Send new message"))
            {
                _main.CharactersLeft = "";
                return;
            }
            int maxChars = chatroom.getUserMaxChars();
            int CharactersLeft = (maxChars - msg.Length);
            if (CharactersLeft < 0)
            {
                _main.CharactersLeft = "max reached!";
                _main.CharactersLeftColor = "Red";
            }
            else
            {
                _main.CharactersLeftColor = "DimGray";
                _main.CharactersLeft = CharactersLeft.ToString();
            }
            _main.MessageContent = msg;
            int currentPosition = msg.Length;
            (sender as TextBox).SelectionStart = currentPosition > 0 ? currentPosition : currentPosition;
        }
        /// <summary>
        /// when the user presses enter, the message is being sent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void send_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button_Send_Click(sender, new RoutedEventArgs());
            }
        }
        #endregion

        #region FOCUS
        /// <summary>
        /// when user clicks on textbox: 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void textBox_NewMessage_gotFocus(object sender, RoutedEventArgs e)
        {
            _main.showNewMsgLbl = false;
        }
        /// <summary>
        /// when user clicks out of textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_NewMessage_LostFocus(object sender, RoutedEventArgs e)
        {
            _main.showNewMsgLbl = true;
        }
        private void lblMouseDown(object sender, RoutedEventArgs e)
        {
            _main.showNewMsgLbl = false;
        }

        #endregion

        /// <summary>
        /// colors the indicator box text
        /// red - error or warning shown to user
        /// black - message or update shown to user
        /// </summary>
        /// <param name="colour"></param>
        private void indicatorColor(string colour)
        {
            if (colour.Equals("red"))
                _main.IndicatorColor = "#ff0000";
            else if (colour.Equals("black"))
                _main.IndicatorColor = "#000000";
        }

        #region BUTTON
        /// <summary>
        /// logout button event - logs out 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Logout(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            chatroom.logout();
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        /// <summary>
        /// edit button event, initializes edit proccess
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edit_Click(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is MessageItem)
            {
                var messageItem = cmd.DataContext as MessageItem;
                var message = messageItem.message as ReadOnlyMessage;
                string nickname = message.nickname;
                if (nickname != loggedInNickname)
                {
                    _pointedMessage = null;
                    _isEdit = false;
                    _main.IndicatorColor = "Red";
                    _main.Indicator = "You cant edit this message.";
                    _main.MessageContent = "";
                }
                else
                {
                    if (messageItem.Equals(_pointedMessage))
                    {
                        messageItem.isSelected = false;
                        _main.IndicatorColor = "Black";
                        _main.Indicator = "";
                        _pointedMessage = null;
                        _isEdit = false;
                        _main.IsTextBoxFocused = false;

                        return;
                    }
                    else if (_pointedMessage != null)
                    {
                        _pointedMessage.isSelected = false;
                        _main.IsTextBoxFocused = false;

                    }
                    _main.IsTextBoxFocused = true;
                    _pointedMessage = messageItem;//save current pointed message
                    _isEdit = true;
                    messageItem.isSelected = true;
                    _main.IndicatorColor = "Blue";
                    _main.Indicator = "If you want to cancel editting, press the message's edit button again.";
                    textBox_NewMessage_gotFocus(sender, e);
                    _main.MessageContent = message.body;
                }
            }
        }
        #endregion 
    }
}

