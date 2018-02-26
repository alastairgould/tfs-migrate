using System;
using System.Linq;
using TfsMigrate.Core.CommitTree;
using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.Exporter
{
    public class GitFastImport : ITraverseCommitTree
    {
        private readonly GitStreamWriter _writer;

        public GitFastImport(GitStreamWriter writer)
        {
            _writer = writer;
        }

        public void ProccessCommit(CommitNode commit)
        {
            commit.AcceptVisitor(this);
        }

        public void VistReset(ResetNode resetNode)
        {
            _writer.WriteLine($"reset ");

            if (resetNode.From != null)
            {
                _writer.Write("from ");
                resetNode.From.AcceptVisitor(this);
                _writer.WriteLine();
            }
        }

        public void VistFileRename(FileRenameNode fileRenameNode)
        {
            _writer.WriteLine($"C \"{fileRenameNode.Source}\" \"{fileRenameNode.Path}\"");
        }

        public void VistFileModify(FileModifyNode fileModifyNode)
        {
            if (fileModifyNode.Blob != null)
            {
                _writer.Write("M 644 ");
                fileModifyNode.Blob.AcceptVisitor(this);
                _writer.Write(" " + "\"" + fileModifyNode.Path + "\"");
                _writer.WriteLine();
            }
            else
            {
                _writer.WriteLine($"M 644 inline \"{fileModifyNode.Path}\"");
                fileModifyNode.Data.AcceptVisitor(this);
            }
        }

        public void VistData(DataNode dataNode)
        {
            var header = $"data {dataNode.Bytes.Result.Length}";
            _writer.WriteLine(header);
            _writer.BaseStream.Write(dataNode.Bytes.Result, 0, dataNode.Bytes.Result.Length);
            _writer.WriteLine();
        }

        public void VistFileDelete(FileDeleteNode dataNode)
        {
            _writer.WriteLine($"D \"{dataNode.Path}\"");
        }

        public void VistDeleteAll(FileDeleteAllNode dataNode)
        {
            _writer.WriteLine("deleteall");
        }

        public void VistFileCopy(FileCopyNode dataNode)
        {
            _writer.WriteLine($"C \"{dataNode.Source}\" \"{dataNode.Path}\"");
        }

        public void VistCommitter(CommitterNode dataNode)
        {
            var command = dataNode.NodeName;

            if (!string.IsNullOrEmpty(dataNode.Name))
            {
                command += " " + dataNode.Name;
            }

            command += $" <{dataNode.Email}> ";
            command += FormatDate(dataNode.Date);

            _writer.WriteLine(command);
        }

        public void VistCommit(CommitNode dataNode)
        {
            foreach (var fc in dataNode.FileNodes.OfType<FileModifyNode>())
            {
                fc.Blob?.MarkNode.AcceptVisitor(this);
            }

            _writer.WriteLine($"commit {dataNode.Reference}");

            if (dataNode.MarkId != null)
            {
                var command = $"mark :{dataNode.MarkId}";
                _writer.WriteLine((command));
                dataNode.HasBeenRendered = true;
            }

            dataNode.Author?.AcceptVisitor(this);

            dataNode.Committer.AcceptVisitor(this);
            dataNode.CommitInfo.AcceptVisitor(this);

            if (dataNode.FromCommit != null)
            {
                _writer.Write("from ");
                dataNode.FromCommit.AcceptVisitor(this);
                _writer.WriteLine();
            }

            foreach (var mc in dataNode.MergeCommits)
            {
                _writer.Write("merge ");
                mc.AcceptVisitor(this);
                _writer.WriteLine();
            }

            foreach (var fc in dataNode.FileNodes)
            {
                fc.AcceptVisitor(this);
            }

            _writer.WriteLine();
        }

        public void VistBlob(BlobNode dataNode)
        {
            if (!dataNode.IsRendered)
            {
                dataNode.IsRendered = true;

                _writer.WriteLine("blob");

                if (dataNode.MarkId != null)
                {
                    var command = $"mark :{dataNode.MarkId}";
                    _writer.WriteLine(command);

                    dataNode.HasBeenRendered = true;
                }

                dataNode.DataNode.AcceptVisitor(this);
            };
        }

        public void VistAuthor(AuthorNode dataNode)
        {
            var command = dataNode.NodeName;

            if (!string.IsNullOrEmpty(dataNode.Name))
            {
                command += " " + dataNode.Name;
            }

            command += $" <{dataNode.Email}> ";
            command += FormatDate(dataNode.Date);

            _writer.WriteLine(command);
        }

        public void VistMarkReference<T>(MarkReferenceNode<T> dataNode) where T : IMarkNode
        {
            if (!dataNode.HasBeenRendered)
            {
                throw new InvalidOperationException("A MarkCommand cannot be referenced if it has not been rendered.");
            }

            var reference = $":{dataNode.MarkId}";
            _writer.Write(reference);
        }

        private static long ToUnixTimestamp(DateTimeOffset dt)
        {
            var unixRef = new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0));
            return (dt.ToUniversalTime().Ticks - unixRef.Ticks) / 10000000;
        }

        private static string FormatDate(DateTimeOffset date)
        {
            var timestamp = ToUnixTimestamp(date);
            return $"{timestamp} +0000";
        }
    }
}
