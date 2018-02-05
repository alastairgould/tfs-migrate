using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class ResetNode : INode
    {
        public string Reference { get; private set; }

        public MarkReferenceNode<CommitNode> From { get; private set; }

        public ResetNode(string reference, MarkReferenceNode<CommitNode> from)
        {
            this.Reference = reference;
            this.From = from;
        }

        public void Vist(ITraverseCommitTree vistor)
        {
            vistor.VistReset(this);
        }
    }
}
