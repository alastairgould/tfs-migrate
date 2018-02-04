using TfsMigrate.Core.CommitTree;

namespace TfsMigrate.Core.GitFastImport
{
    public interface IVistor
    {
        void VistReset(ResetNode resetNode);
        void VistCommit(CommitNode nameNode);
        void VistFileRename(FileRenameNode fileRenameNode);
        void VistFileModify(FileModifyNode fileModifyNode);
        void VistData(DataNode dataNode);
        void VistFileDelete(FileDeleteNode dataNode);
        void VistDeleteAll(FileDeleteAllNode dataNode);
        void VistFileCopy(FileCopyNode dataNode);
        void VistCommitter(CommitterNode dataNode);
        void VistBlob(BlobNode dataNode);
        void VistAuthor(AuthorNode dataNode);
        void VistMarkReference(MarkReferenceNode dataNode);
    }
}