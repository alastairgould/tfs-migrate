using System.Collections.ObjectModel;
using System.Text;
using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class DataNode : INode
    {
        internal byte[] Bytes;

        public DataNode(byte[] bytes)
        {
            Bytes = (byte[])bytes.Clone();
        }

        public DataNode(string str)
        {
            Bytes = Encoding.UTF8.GetBytes(str);
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistData(this);
        }
    }
}
