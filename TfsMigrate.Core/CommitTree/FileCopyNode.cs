using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class FileCopyNode : IFileNode
    {
        public string Source { get; }

        public string Path { get; }

        public FileCopyNode(string src, string dest)
        {
            Source = src;
            Path = dest;
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistFileCopy(this);
        }
    }
}
