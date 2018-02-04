using TfsMigrate.Core.GitFastImport;

namespace TfsMigrate.Core.CommitTree
{
    public class FileDeleteNode : IFileNode
    {
        public string Path { get; private set; } 
        public FileDeleteNode(string path)
        {
            this.Path = path;
        }

        public void Vist(IVistor vistor)
        {
            vistor.VistFileDelete(this);
        }
    }
}
