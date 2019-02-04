using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    interface IQueryAction
    {
        /// <summary>
        /// executes the string chaining
        /// </summary>
        /// <returns></returns>
        void execute(Query query);
    }
}
