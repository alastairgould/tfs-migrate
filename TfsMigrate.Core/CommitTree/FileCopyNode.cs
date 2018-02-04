using TfsMigrate.Core.GitFastImport;

namespace TfsMigrate.Core.CommitTree
{
    public class FileCopyNode : IFileNode
    {
        public string Source { get; private set; }

        public string Path { get; private set; }

        public FileCopyNode(string src, string dest)
        {
            this.Source = src;
            this.Path = dest;
        }

        public void Vist(IVistor vistor)
        {
            vistor.VistFileCopy(this);
        }
    }
}
