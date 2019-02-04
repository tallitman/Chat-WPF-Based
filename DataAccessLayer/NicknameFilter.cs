using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    class NicknameFilter : IQueryAction
    {
        private string _nickname = "";
        public string nickname { get { return _nickname; } }
        public NicknameFilter(String nick)
        {
            _nickname = nick;
        }
        public void execute(Query query)
        {
            query.addFilter("AND[Users].Nickname = @Nickname ");
            query.nickname = nickname;
        }

    }
}
