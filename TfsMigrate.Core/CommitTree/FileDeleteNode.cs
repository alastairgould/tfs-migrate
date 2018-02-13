using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class FileDeleteNode : IFileNode
    {
        public string Path { get; }

        public FileDeleteNode(string path)
        {
            Path = path;
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistFileDelete(this);
        }
    }
}
