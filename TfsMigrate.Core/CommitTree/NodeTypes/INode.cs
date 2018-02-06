using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree.NodeTypes
{
    public interface INode
    {
        void AcceptVisitor(ITraverseCommitTree vistor);
    }
}
