using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public interface INode
    {
        void Vist(ITraverseCommitTree vistor);
    }
}
