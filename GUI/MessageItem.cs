using BusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    /// <summary>
    /// This class represents the Object insert to the ListBox in ChatRoom
    /// </summary>
    public class MessageItem : IEquatable<MessageItem> , INotifyPropertyChanged
    {
        #region PROPERTIES
        private ReadOnlyMessage _message;
        public ReadOnlyMessage message { get { return _message; } }
        private string _buttonVisibility;
        public string buttonVisibility { get { return _buttonVisibility; } }
        private bool _isSelected = false;
        public bool isSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                changeIcon();
            }
        }
        private string _iconPath = "images/pencil1.png";
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public MessageItem(ReadOnlyMessage roMSG, bool isVisible)
        {
            _message = roMSG;
            if (isVisible)
                _buttonVisibility = "Visible";
            else
                _buttonVisibility = "Hidden";
        }


        /// <summary>
        /// the directory path of the current item's icon
        /// </summary>
        public string iconPath { get { return _iconPath; } set { _iconPath = value; OnPropertyChanged("iconPath"); } }

        /// <summary>
        /// changes the icon according to the selection
        /// </summary>
        private void changeIcon()
        {
            if (!isSelected)
                iconPath = "images/pencil1.png";
            else
                iconPath = "images/pencil2.png";

        }

        /// <summary>
        /// checks if given message item equals to current
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MessageItem other)
        {
            if (other == null) return false;
            return message.Equals(other.message);

        }

        /// <summary>
        /// property changed notifier
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
