using TfsMigrate.Core.GitFastImport;

namespace TfsMigrate.Core.CommitTree
{
    public class FileModifyNode : IFileNode
    {
        public string Path { get; private set; }
        public MarkReferenceNode Blob { get; private set; }
        public FileModifyNode(string path, MarkReferenceNode blob)
        {
            this.Path = path;
            this.Blob = blob;
        }

        public DataNode Data { get; private set; }
        public FileModifyNode(string path, byte[] data)
        {
            this.Path = path;
            this.Data = new DataNode(data);
        }

        public void Vist(IVistor vistor)
        {
            vistor.VistFileModify(this);
        }
    }
}
