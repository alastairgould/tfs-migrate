using TfsMigrate.Core.GitFastImport;

namespace TfsMigrate.Core.CommitTree
{
    public class FileDeleteAllNode : IFileNode
    {
        public string Path { get; private set; } 

        public FileDeleteAllNode()
        {
            this.Path = null;
        }

        public void Vist(IVistor vistor)
        {
            vistor.VistDeleteAll(this);
        }
    }
}
