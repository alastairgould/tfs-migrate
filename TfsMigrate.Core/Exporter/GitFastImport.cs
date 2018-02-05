using System;
using System.Linq;
using TfsMigrate.Core.CommitTree;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.Exporter
{
    public class GitFastImport : ITraverseCommitTree
    {
        private GitStreamWriter writer;

        public GitFastImport(GitStreamWriter writer)
        {
            this.writer = writer;
        }

        public void ProccessCommit(CommitNode commit)
        {
            commit.Vist(this);
        }

        public void VistReset(ResetNode resetNode)
        {
            writer.WriteLine(string.Format("reset {0}", resetNode.Reference));

            if (resetNode.From != null)
            {
                writer.Write("from ");
                resetNode.From.Vist(this);
                writer.WriteLine();
            }
        }

        public void VistFileRename(FileRenameNode fileRenameNode)
        {
            writer.WriteLine(string.Format("C {0} {1}", fileRenameNode.Source, fileRenameNode.Path));
        }

        public void VistFileModify(FileModifyNode fileModifyNode)
        {
            if (fileModifyNode.Blob != null)
            {
                writer.Write("M 644 ");
                fileModifyNode.Blob.Vist(this);
                writer.Write(" " + fileModifyNode.Path);
                writer.WriteLine();
            }
            else
            {
                writer.WriteLine(string.Format("M 644 inline {0}", fileModifyNode.Path));
                fileModifyNode.Data.Vist(this);
            }
        }

        public void VistData(DataNode dataNode)
        {
            var header = string.Format("data {0}", dataNode._Bytes.Length);
            writer.WriteLine(header);
            writer.BaseStream.Write(dataNode._Bytes, 0, dataNode._Bytes.Length);
            writer.WriteLine();
        }

        public void VistFileDelete(FileDeleteNode dataNode)
        {
            writer.WriteLine(string.Format("D {0}", dataNode.Path));
        }

        public void VistDeleteAll(FileDeleteAllNode dataNode)
        {
            writer.WriteLine("deleteall");
        }

        public void VistFileCopy(FileCopyNode dataNode)
        {
            writer.WriteLine(string.Format("C {0} {1}", dataNode.Source, dataNode.Path));
        }

        public void VistCommitter(CommitterNode dataNode)
        {
            var command = dataNode.NodeName;

            if (!string.IsNullOrEmpty(dataNode.Name))
            {
                command += " " + dataNode.Name;
            }

            command += string.Format(" <{0}> ", dataNode.Email);
            command += FormatDate(dataNode.Date);

            writer.WriteLine(command);
        }

        public void VistCommit(CommitNode dataNode)
        {
            foreach (var fc in dataNode.FileNodes.OfType<FileModifyNode>())
            {
                if (fc.Blob != null)
                {
                    fc.Blob.MarkNode.Vist(this);
                }
            }

            writer.WriteLine(string.Format("commit {0}", dataNode.Reference));

            if (dataNode.MarkId != null)
            {
                var command = string.Format("mark :{0}", dataNode.MarkId);
                writer.WriteLine((command));
                dataNode.HasBeenRendered = true;
            }

            if (dataNode.Author != null)
            {
                dataNode.Author.Vist(this);
            }

            dataNode.Committer.Vist(this);
            dataNode.CommitInfo.Vist(this);

            if (dataNode.FromCommit != null)
            {
                writer.Write("from ");
                dataNode.FromCommit.Vist(this);
                writer.WriteLine();
            }

            foreach (var mc in dataNode.MergeCommits)
            {
                writer.Write("merge ");
                mc.Vist(this);
                writer.WriteLine();
            }

            foreach (var fc in dataNode.FileNodes)
            {
                fc.Vist(this);
            }

            writer.WriteLine();
        }

        public void VistBlob(BlobNode dataNode)
        {
            if (!dataNode.IsRendered)
            {
                dataNode.IsRendered = true;

                writer.WriteLine("blob");

                if (dataNode.MarkId != null)
                {
                    var command = string.Format("mark :{0}", dataNode.MarkId);
                    writer.WriteLine(command);

                    dataNode.HasBeenRendered = true;
                }

                dataNode.DataNode.Vist(this);
            };
        }

        public void VistAuthor(AuthorNode dataNode)
        {
            var command = dataNode.NodeName;

            if (!string.IsNullOrEmpty(dataNode.Name))
            {
                command += " " + dataNode.Name;
            }

            command += string.Format(" <{0}> ", dataNode.Email);
            command += FormatDate(dataNode.Date);

            writer.WriteLine(command);
        }

        public void VistMarkReference<T>(MarkReferenceNode<T> dataNode) where T : IMarkNode
        {
            if (!dataNode.HasBeenRendered)
            {
                throw new InvalidOperationException("A MarkCommand cannot be referenced if it has not been rendered.");
            }

            var reference = string.Format(":{0}", dataNode.MarkId);
            writer.Write(reference);
        }

        private static long ToUnixTimestamp(DateTimeOffset dt)
        {
            DateTimeOffset unixRef = new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0));
            return (dt.ToUniversalTime().Ticks - unixRef.Ticks) / 10000000;
        }

        private static string FormatDate(DateTimeOffset date)
        {
            var timestamp = ToUnixTimestamp(date);
            return string.Format("{0} +0000", timestamp);
        }
    }
}
