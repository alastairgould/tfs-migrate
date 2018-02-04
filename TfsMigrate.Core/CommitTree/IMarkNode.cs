namespace TfsMigrate.Core.CommitTree
{
    public interface IMarkNode : INode
    {
        int? MarkId { get; }
        bool HasBeenRendered { get; set; }
    }
}
