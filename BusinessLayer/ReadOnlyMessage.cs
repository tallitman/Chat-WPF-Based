using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
   public class ReadOnlyMessage : IEquatable<ReadOnlyMessage>
    {
        #region properties       
        private string _body;            //Message content
        public string body
        {
            get { return _body; }
        }
        private string _nickname;        //The user who sent the message
        public string nickname
        {
            get { return _nickname; }
        }
        private string _g_Id;
        public string g_ID
        {
            get { return _g_Id; }
        }
        private Guid _GUID;               //Message Guid
        public Guid GUID
        {
            get { return _GUID; }
        }
        private DateTime _date;           //Message's date
        public DateTime date
        {
            get { return _date; }
        }
        #endregion

        public ReadOnlyMessage(Guid GUID, DateTime date, string nickname, string g_ID, string body)
        {
            _date = date;
            _GUID = GUID;
            _nickname = nickname;
            _g_Id = g_ID;
            _body = body;
        }
        public override string ToString()
        {
            string ans = String.Format("[{1} - {0} ({3})] : {2}", nickname, date.ToString(), body, g_ID);
            return ans;
        }
        /// <summary>
        /// equals by GUID and date implementation
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ReadOnlyMessage other)
        {
            if (other == null) return false;
            return GUID == other.GUID && _date == other.date;

        }
    }
}
