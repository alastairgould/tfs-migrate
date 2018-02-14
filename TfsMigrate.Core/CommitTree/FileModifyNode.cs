using System.Threading.Tasks;
using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class FileModifyNode : IFileNode
    {
        public string Path { get; }

        public MarkReferenceNode<BlobNode> Blob { get; }

        public DataNode Data { get; }

        public FileModifyNode(string path, MarkReferenceNode<BlobNode> blob)
        {
            Path = path;
            Blob = blob;
        }

        public FileModifyNode(string path, Task<byte[]> data)
        {
            Path = path;
            Data = new DataNode(data);
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistFileModify(this);
        }
    }
}
