using TfsMigrate.Core.GitFastImport;

namespace TfsMigrate.Core.CommitTree
{
    public class FileRenameNode : IFileNode
    {
        public string Path { get; private set; }
        public string Source { get; private set; }
        public FileRenameNode(string src, string dest)
        {
            this.Source = src;
            this.Path = dest;
        }

        public void Vist(IVistor vistor)
        {
            vistor.VistFileRename(this);
        }
    }
}
