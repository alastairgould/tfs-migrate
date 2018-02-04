using TfsMigrate.Core.GitFastImport;

namespace TfsMigrate.Core.CommitTree
{
    public interface INode
    {
        void Vist(IVistor vistor);
    }
}
