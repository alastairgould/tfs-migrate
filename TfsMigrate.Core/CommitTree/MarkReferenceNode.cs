using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class MarkReferenceNode<T> : INode where T: IMarkNode
    {
        public int? MarkId { get { return MarkNode.MarkId; } }

        public bool HasBeenRendered { get { return MarkNode.HasBeenRendered; } }

        public T MarkNode { get; set;}

        public MarkReferenceNode(T markNode)
        {
            MarkNode = markNode;
        }

        public void Vist(ITraverseCommitTree vistor)
        {
            vistor.VistMarkReference(this);
        }
    }
}
