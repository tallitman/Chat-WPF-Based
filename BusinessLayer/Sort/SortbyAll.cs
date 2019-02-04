
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Sort
{
    public class SortbyAll : ISort
    {
        /// <summary>
        /// specifies whether the sorting is ascending or descending.
        /// </summary>
        private bool _ascending;
       public SortbyAll(bool ascending)
        {
            _ascending = ascending;
        }
        public List<Message> DoAction(List<Message> messages)
        {
            string res = "";
            List<Message> sortedMessages;
            if (_ascending)
            {
                var sortedMsg = (from msg in messages
                                 orderby Convert.ToInt32(msg.g_ID) ascending, msg.nickname ascending, msg.date ascending
                                 select msg);
                sortedMessages = sortedMsg.ToList() ;
                foreach (Message msg in sortedMsg)
                    res += msg + "\n";
            }
            else
            {
                var sortedMsg = (from msg in messages
                                 orderby Convert.ToInt32(msg.g_ID) descending, msg.nickname descending, msg.date descending
                                 select msg);
                sortedMessages = sortedMsg.ToList();
                foreach (Message msg in sortedMsg)
                    res += msg + "\n";
            }
            return sortedMessages;
        }
    }
}
