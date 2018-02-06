using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class ResetNode : INode
    {
        public string Reference { get; }

        public MarkReferenceNode<CommitNode> From { get; }

        public ResetNode(string reference, MarkReferenceNode<CommitNode> from)
        {
            this.Reference = reference;
            this.From = from;
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistReset(this);
        }
    }
}
