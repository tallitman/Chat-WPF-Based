using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Automation.Peers;
using BusinessLayer;

namespace GUI
{
    public class ObrModels : INotifyPropertyChanged
    {
        #region COLLECTIONS
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<MessageItem> _Messages = new ObservableCollection<MessageItem>();
        public ObservableCollection<MessageItem> Messages 
        {
            get
            {
                return _Messages;
            }
            set
            {
                _Messages = value;
                OnPropertyChanged("Messages");
            }
        } 
        public ObservableCollection<string> FilterByGidList { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> FilterByGidNickname { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> g_id_selection { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> FilterByItems { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> SortByItems { get; } = new ObservableCollection<string>();
        List<KeyValuePair<string, string>> usersList = new List<KeyValuePair<string, string>>();
        #endregion

        #region LOGIN
        //Determines the appearance of the login/register window button and texts
        private string _logRegStatus = "Login";
        public string logRegStatus { get { return _logRegStatus; } set { _logRegStatus = value; OnPropertyChanged("logRegStatus"); } }

        private string _logRegBtnStatus = "Log in";
        public string logRegBtnStatus { get { return _logRegBtnStatus; } set { _logRegBtnStatus = value; OnPropertyChanged("logRegBtnStatus"); } }

        //focus of send textbox
        private bool _IsTextBoxFocused = false;
        public bool IsTextBoxFocused { get { return _IsTextBoxFocused; } set { _IsTextBoxFocused = value; OnPropertyChanged("IsTextBoxFocused"); } }

        //selected g_id in ComboBox
        private string _g_id_selected = "Choose your Group ID";
        public string g_id_selected
        {
            get { return _g_id_selected; }
            set
            {
                _g_id_selected = value;

                OnPropertyChanged(value);
            }
        }

        //userName TextBox
        private string _user_name = "Type here your user name";
        public string user_name
        {
            get { return _user_name; }
            set
            {
                _user_name = value;
                OnPropertyChanged("user_name");
            }
        }

        #endregion

        #region CHATROOM


        //Server Time string
        private string _serverTime = "Server Time :";
        public string ServerTime
        {
            get
            {
                return _serverTime;
            }
            set
            {
                _serverTime = "Server Time : " + value;
                OnPropertyChanged("ServerTime");
            }
        }


        //FilterBy ComboBox
        private string _FilterByCombobox = "None";
        public string FilterByCombobox
        {
            get { return _FilterByCombobox; }
            set
            {
                _FilterByCombobox = value;
                filterByMenuChanged();
                OnPropertyChanged("FilterByIndex");
            }
        }

        //Hidden or Visible FilterBy menus
        private string _FilterGidView = "Hidden";
        public string FilterGidView
        {
            get { return _FilterGidView; }
            set
            {
                _FilterGidView = value;
                OnPropertyChanged("FilterGidView");
            }
        }
        private string _FilterNicknameView = "Hidden";
        public string FilterNicknameView
        {
            get { return _FilterNicknameView; }
            set
            {
                _FilterNicknameView = value;
                OnPropertyChanged("FilterNicknameView");
            }
        }

        //FilterBy Gid
        private string _filterByGid = "";
        public string FilterByGid
        {
            get { return _filterByGid; }
            set
            {
                _filterByGid = value;
                setItemsNickname();
                _filterByNickname = "";
                OnPropertyChanged("FilterByGid");
            }
        }

        //FilterBy Nickname
        private string _filterByNickname = "";
        public string FilterByNickname
        {
            get { return _filterByNickname; }
            set
            {
                _filterByNickname = value;
                OnPropertyChanged("FilterByNickname");
            }
        }
       
        public void scroll()
        {
            
           // Messages = observableCollection;
            MessageItem tmp = Messages.Last();
            Messages.Remove(tmp);
            Messages.Add(tmp);
            MessagesSelectedIndex = Messages.Count - 1;
        }

        // Ascending & Descending radio buttons
        private bool _Ascending = true;
        public bool Ascending
        {
            get { return _Ascending; }
            set
            {
                if (_Ascending == false)
                {
                    _Ascending = !_Ascending; _Descending = !_Descending;
                    OnPropertyChanged("Ascending"); OnPropertyChanged("Descending");
                }
            }
        }
        private bool _Descending = false;
        public bool Descending
        {
            get { return _Descending; }
            set
            {
                if (_Descending == false)
                {
                    _Ascending = !_Ascending; _Descending = !_Descending;
                    OnPropertyChanged("Ascending"); OnPropertyChanged("Descending");
                }
            }
        }

        //SortBy ComboBox
        private string _SortByCombobox = "Timestamp";
        public string SortByCombobox
        {
            get { return _SortByCombobox; }
            set
            {
                _SortByCombobox = value;
                OnPropertyChanged("SortByIndex");
            }
        }


        //Characters Left TextBox
        private string _CharactersLeft = "";
        public string CharactersLeft
        {
            get { return _CharactersLeft; }
            set
            {
                _CharactersLeft = value;
                OnPropertyChanged("CharactersLeft");
            }
        }
        //Send new message 
        private bool _showNewMsgLbl = true;
        private string _sendNewMessage = "Visible";
        public bool showNewMsgLbl
        {
            get
            {
                return _showNewMsgLbl;
            }
            set
            {
                _showNewMsgLbl = value;
                if (_showNewMsgLbl == false)
                {
                    sendNewMessage = "Hidden";

                }
                else
                    if (MessageContent.Equals(""))
                    sendNewMessage = "Visibilty";

            }
        }
        public string sendNewMessage
        {
            get
            {
                return _sendNewMessage;
            }
            set
            {
                _sendNewMessage = value;
                OnPropertyChanged("sendNewMessage");
            }
        }


        //Characters Left Color
        private string _CharactersLeftColor = "DimGray";
        public string CharactersLeftColor
        {
            get { return _CharactersLeftColor; }
            set
            {
                _CharactersLeftColor = value;
                OnPropertyChanged("CharactersLeftColor");
            }
        }

        //Message TextBox
        private string _messageContent = "";
        public string MessageContent
        {
            get { return _messageContent; }
            set
            {
                _messageContent = value;
                OnPropertyChanged("MessageContent");
            }
        }

        // Logged in as Label
        private string _loggedInUser = "";
        public string LoggedInContent
        {
            get { return _loggedInUser; }
            set
            {
                //   _loggedInUser = "Logged in as: " + value;
                _loggedInUser = value;
                OnPropertyChanged("LoggedInContent");
            }
        }

        // Indicator TextBox
        /// <summary>
        /// indicator changes according user's clicks and choosings,
        /// indicator color can be change due to type of message shown to user
        /// red symbolizes an alert or error
        /// black symbolizes an update
        /// </summary>
        private string _indicator = "";
        public string Indicator
        {
            get { return _indicator; }
            set
            {
                _indicator = value;
                OnPropertyChanged("Indicator");
            }
        }

        //text colour
        private string _indicatorColor = "#000000";
        public string IndicatorColor
        {
            get { return _indicatorColor; }
            set
            {
                _indicatorColor = value;
                OnPropertyChanged("IndicatorColor");
            }
        }

        //chatroom listbox 
        private int _messagesSelectedIndex = 0;
        public int MessagesSelectedIndex
        {
            get { return _messagesSelectedIndex; }
            set
            {
                _messagesSelectedIndex = value;
                OnPropertyChanged("MessagesSelectedIndex");
            }
        }
        
        #endregion

        #region INITIALIZERS & CONSTRUCTOR
        /// <summary>
        /// constructor, adds items to lists
        /// </summary>
        public ObrModels()
        {
            Messages.CollectionChanged += Messages_CollectionChanged;
            setItemsFilterBy();
            setItemsGid();
            setItemSortBy();
        }
        /// <summary>
        /// insert items to filterBy combobox
        /// </summary>
        private void setItemsFilterBy()
        {
            FilterByItems.Add("None");
            FilterByItems.Add("Group");
            FilterByItems.Add("User");
        }
        /// <summary>
        /// insert items to GID combobox
        /// </summary>
        private void setItemsGid()
        {
            for (int i = 1; i <= 40; i++)
            {
                FilterByGidList.Add(i.ToString());
            }
        }
        /// <summary>
        /// insert items to nickname combobox
        /// </summary>
        /// <param name="users"></param>
        public void setItemsNickname(List<KeyValuePair<string, string>> users)
        {
            usersList = users;
        }
        public void setItemsNickname()
        {
            FilterByGidNickname.Clear();
            foreach (KeyValuePair<string, string> pair in usersList)
            {
                if (pair.Key == FilterByGid)
                    FilterByGidNickname.Add(pair.Value);
            }
        }

        /// <summary>
        /// inserts gid numbers onto combobox
        /// </summary>
        /// <param name="s"></param>
        public void setGidItemsInComboBox(string s)
        {
            g_id_selection.Add(s);

        }
        /// <summary>
        /// inserts items to sortby combobox
        /// </summary>
        public void setItemSortBy()

        {
            SortByItems.Add("Timestamp");
            SortByItems.Add("Nickname");
            SortByItems.Add("Group, Nickname and Timestamp");
        }
        #endregion

        #region MODIFIERS & UPDATERS
        /// <summary>
        /// whenever one of the filterby ComboBox items is chosen, options are shown
        /// </summary>
        private void filterByMenuChanged()
        {
            if (FilterByCombobox == "None")
            {
                FilterGidView = "Hidden";
                FilterNicknameView = "Hidden";
            }
            else if (FilterByCombobox == "Group")
            {
                FilterGidView = "Visible";
                FilterNicknameView = "Hidden";
            }
            else if (FilterByCombobox == "User")
            {
                FilterGidView = "Visible";
                FilterNicknameView = "Visible";
            }
        }
        /// <summary>
        /// updates the message list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            MessagesSelectedIndex = Messages.Count-1;
            OnPropertyChanged("Messages");
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}