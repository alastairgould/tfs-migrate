using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsMigrate.Core.CommitTree;

namespace TfsMigrate.Core.Tfs
{
    public class Branches
    {
        private Dictionary<string, Tuple<string, CommitNode>> _Branches = new Dictionary<string, Tuple<string, CommitNode>>();

        public Branches()
        {

        }
    }
}
