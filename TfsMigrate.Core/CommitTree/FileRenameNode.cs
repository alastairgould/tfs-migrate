using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class FileRenameNode : IFileNode
    {
        public string Path { get; }

        public string Source { get; }

        public FileRenameNode(string src, string dest)
        {
            this.Source = src;
            this.Path = dest;
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistFileRename(this);
        }
    }
}
