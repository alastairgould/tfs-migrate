namespace TfsMigrate.Core.CommitTree.NodeTypes
{
    public interface IMarkNode : INode
    {
        int? MarkId { get; }

        bool HasBeenRendered { get; set; }
    }
}
