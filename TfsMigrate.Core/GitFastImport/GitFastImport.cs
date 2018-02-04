using System;
using System.IO;
using System.Linq;
using System.Text;
using TfsMigrate.Core.CommitTree;

namespace TfsMigrate.Core.GitFastImport
{
    public class GitFastImport : IGitFastImport
    {
        private Stream stream;

        private byte[] _LineFeed = new byte[] {0x0A};

        public GitFastImport(Stream stream)
        {
            this.stream = stream;
        }

        public void ProccessCommit(CommitNode commit)
        {
            commit.Vist(this);
        }

        public void VistReset(ResetNode resetNode)
        {
            WriteLine(string.Format("reset {0}", resetNode.Reference));

            if (resetNode.From != null)
            {
                WriteString("from ");
                resetNode.From.Vist(this);
                WriteLineFeed();
            }
        }

        public void VistFileRename(FileRenameNode fileRenameNode)
        {
            WriteLine(string.Format("C {0} {1}", fileRenameNode.Source, fileRenameNode.Path));
        }

        public void VistFileModify(FileModifyNode fileModifyNode)
        {
            if (fileModifyNode.Blob != null)
            {
                WriteString("M 644 ");
                fileModifyNode.Blob.Vist(this);
                WriteString(" " + fileModifyNode.Path);
                WriteLineFeed();
            }
            else
            {
                WriteLine(string.Format("M 644 inline {0}", fileModifyNode.Path));
                fileModifyNode.Data.Vist(this);

            }
        }

        public void VistData(DataNode dataNode)
        {
            var header = string.Format("data {0}", dataNode._Bytes.Length);
            WriteLine(header);
            stream.Write(dataNode._Bytes, 0, dataNode._Bytes.Length);
            WriteLineFeed();
        }

        public void VistFileDelete(FileDeleteNode dataNode)
        {
            WriteLine(string.Format("D {0}", dataNode.Path));
        }

        public void VistDeleteAll(FileDeleteAllNode dataNode)
        {
            WriteLine("deleteall");
        }

        public void VistFileCopy(FileCopyNode dataNode)
        {
            WriteLine(string.Format("C {0} {1}", dataNode.Source, dataNode.Path));
        }

        public void VistCommitter(CommitterNode dataNode)
        {
            var command = dataNode.CommandName;
            if (!string.IsNullOrEmpty(dataNode.Name))
                command += " " + dataNode.Name;
            command += string.Format(" <{0}> ", dataNode.Email);
            command += FormatDate(dataNode.Date);

            WriteLine(command);
        }

        public void VistCommit(CommitNode dataNode)
        {
            foreach (var fc in dataNode.FileNodes.OfType<FileModifyNode>())
            {
                if (fc.Blob != null)
                    fc.Blob.MarkNode.Vist(this);
            }

            WriteLine(string.Format("commit {0}", dataNode.Reference));

            if (dataNode.MarkId != null)
            {
                var command = string.Format("mark :{0}", dataNode.MarkId);
                WriteLine(command);
                dataNode.HasBeenRendered = true;
            }

            if (dataNode.Author != null)
            dataNode.Author.Vist(this);

            dataNode.Committer.Vist(this);
            dataNode.CommitInfo.Vist(this);

            if (dataNode.FromCommit != null)
            {
                WriteString("from ");
                dataNode.FromCommit.Vist(this);
                WriteLineFeed();
            }

            foreach (var mc in dataNode.MergeCommits)
            {
                WriteString("merge ");
                mc.Vist(this);
                WriteLineFeed();
            }

            foreach (var fc in dataNode.FileNodes)
                fc.Vist(this);

            WriteLineFeed();
        }

        public void VistBlob(BlobNode dataNode)
        {
            if (!dataNode.IsRendered)
            {
                dataNode.IsRendered = true;

                WriteLine("blob");

                if (dataNode.MarkId != null)
                {
                    var command = string.Format("mark :{0}", dataNode.MarkId);
                    WriteLine(command);

                    dataNode.HasBeenRendered = true;
                }

                dataNode.DataNode.Vist(this);
            };
        }

        public void VistAuthor(AuthorNode dataNode)
        {
            var command = dataNode.CommandName;
            if (!string.IsNullOrEmpty(dataNode.Name))
                command += " " + dataNode.Name;
            command += string.Format(" <{0}> ", dataNode.Email);
            command += FormatDate(dataNode.Date);

            WriteLine(command);
        }
        public void VistMarkReference(MarkReferenceNode dataNode)
        {
            if (!dataNode.HasBeenRendered)
                throw new InvalidOperationException("A MarkCommand cannot be referenced if it has not been rendered.");

            var reference = string.Format(":{0}", dataNode.MarkId);
            WriteString(reference);
        }

        private void OutputMark(IMarkNode markNode)
        {
            if (markNode.MarkId != null)
            {
                var command = string.Format("mark :{0}", markNode.MarkId);
                WriteLine(command);
                markNode.HasBeenRendered = true;
            }
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

        private void WriteLine(string s)
        {
            WriteString(s);
            WriteLineFeed();
        }

        private void WriteString(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            stream.Write(bytes, 0, bytes.Length);
        }

        private void WriteLineFeed()
        {
            stream.Write(_LineFeed, 0, 1);
        }
    }
}
