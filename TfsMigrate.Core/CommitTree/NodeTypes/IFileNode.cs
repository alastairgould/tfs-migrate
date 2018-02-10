namespace TfsMigrate.Core.CommitTree.NodeTypes
{
    public interface IFileNode : INode
    {
        string Path { get; }
    }
}
