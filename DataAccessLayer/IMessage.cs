using System;

namespace DataAccessLayer
{
    /// <summary>
    /// Message object which is able from DataAccess
    /// </summary>
    public class IMessage
    {
        public Guid _Id { get; }
        public string _UserName { get; }
        public DateTime _Date { get; }
        public string _MessageContent { get; }
        public string _GroupID { get; }
        public IMessage(Guid id , string groupId, string userName, string messageContent, DateTime utcTime)
        {
            _Id = id;
            _UserName = userName;
            _Date = utcTime;
            _MessageContent = messageContent;
            _GroupID = groupId;
        }
        public override string ToString()
        {
            return _MessageContent + "\n" + _Id;
        }
    }
}