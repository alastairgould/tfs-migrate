using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class FileDeleteNode : IFileNode
    {
        public string Path { get; private set; }

        public FileDeleteNode(string path)
        {
            this.Path = path;
        }

        public void Vist(ITraverseCommitTree vistor)
        {
            vistor.VistFileDelete(this);
        }
    }
}
