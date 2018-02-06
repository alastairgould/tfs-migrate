using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class MarkReferenceNode<T> : INode where T: IMarkNode
    {
        public int? MarkId => MarkNode.MarkId;

        public bool HasBeenRendered => MarkNode.HasBeenRendered;

        public T MarkNode { get; set;}

        public MarkReferenceNode(T markNode)
        {
            MarkNode = markNode;
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistMarkReference(this);
        }
    }
}
