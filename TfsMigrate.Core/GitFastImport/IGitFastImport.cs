using TfsMigrate.Core.CommitTree;

namespace TfsMigrate.Core.GitFastImport
{
    public interface IGitFastImport : IVistor
    {
        void ProccessCommit(CommitNode commit);
    }
}
