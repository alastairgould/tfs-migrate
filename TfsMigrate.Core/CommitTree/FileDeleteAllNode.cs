using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class FileDeleteAllNode : IFileNode
    {
        public string Path { get; private set; } 

        public FileDeleteAllNode()
        {
            this.Path = null;
        }

        public void Vist(ITraverseCommitTree vistor)
        {
            vistor.VistDeleteAll(this);
        }
    }
}
