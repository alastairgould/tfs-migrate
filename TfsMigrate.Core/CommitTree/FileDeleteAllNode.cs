using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class FileDeleteAllNode : IFileNode
    {
        public string Path { get; } 

        public FileDeleteAllNode()
        {
            Path = null;
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistDeleteAll(this);
        }
    }
}
