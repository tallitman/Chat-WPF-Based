using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    class GroupFilter : IQueryAction
    {
        private int _gid;
        public int gid { get { return _gid; } }
        public GroupFilter(int group)
        {
            _gid = group;
        }
        public void execute(Query query)
        {
            query.addFilter("AND[Users].Group_Id = @GroupId ");
            query.gid = gid;
        }

    }
}
