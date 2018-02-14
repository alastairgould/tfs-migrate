using System.Text;
using System.Threading.Tasks;
using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class DataNode : INode
    {
        internal Task<byte[]> Bytes;

        public DataNode(Task<byte[]> bytes)
        {
            Bytes = bytes;
        }

        public DataNode(string str)
        {
            Bytes = Task.Run(() => Encoding.UTF8.GetBytes(str));
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistData(this);
        }
    }
}
