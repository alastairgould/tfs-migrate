using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class FileModifyNode : IFileNode
    {
        public string Path { get; private set; }

        public MarkReferenceNode<BlobNode> Blob { get; private set; }

        public DataNode Data { get; private set; }

        public FileModifyNode(string path, MarkReferenceNode<BlobNode> blob)
        {
            this.Path = path;
            this.Blob = blob;
        }

        public FileModifyNode(string path, byte[] data)
        {
            this.Path = path;
            this.Data = new DataNode(data);
        }

        public void Vist(ITraverseCommitTree vistor)
        {
            vistor.VistFileModify(this);
        }
    }
}
