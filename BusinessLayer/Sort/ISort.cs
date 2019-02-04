
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Sort
{
    public interface ISort
    {
        /// <summary>
        /// receives a list of messages and does the operation of sorting.
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        List<Message> DoAction(List<Message> messages);
    }
}