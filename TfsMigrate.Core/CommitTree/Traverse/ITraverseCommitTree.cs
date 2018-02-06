namespace TfsMigrate.Core.CommitTree.Traverse
{
    public interface ITraverseCommitTree
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
        void VistMarkReference<T>(MarkReferenceNode<T> dataNode) where T: IMarkNode;
    }
}