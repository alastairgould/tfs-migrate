using System;
using System.Collections.Generic;
using System.Linq;
using TfsMigrate.Core.GitFastImport;

namespace TfsMigrate.Core.CommitTree
{
    public class CommitNode : IMarkNode
    {
        public string Reference { get; private set; }
        public AuthorNode Author { get; private set; }
        public CommitterNode Committer { get; private set; }
        public DataNode CommitInfo { get; private set; }
        public MarkReferenceNode FromCommit { get; private set; }
        public IList<MarkReferenceNode> MergeCommits { get; private set; }
        public IList<IFileNode> FileNodes { get; private set; }
        public int? MarkId { get; private set; }
        public bool HasBeenRendered { get; set; }

        public CommitNode(
            int markId,
            string reference,
            AuthorNode author,
            CommitterNode committer,
            DataNode commitInfo,
            MarkReferenceNode fromCommit,
            IList<MarkReferenceNode> mergeCommits,
            IList<IFileNode> fileNodes)
        {
            if (string.IsNullOrEmpty(reference))
                throw new InvalidOperationException("The Reference for this commit must be valid.");
            if (committer == null)
                throw new InvalidOperationException("A committer must be specified for this commit.");
            if (commitInfo == null)
                throw new InvalidOperationException("Commit Information must be specified for this commit.");

            this.MarkId = markId;
            this.Reference = reference;
            this.Author = author;
            this.Committer = committer;
            this.CommitInfo = commitInfo;
            this.FromCommit = fromCommit;
            this.MergeCommits = (mergeCommits ?? new List<MarkReferenceNode>()).ToList().AsReadOnly();
            this.FileNodes = (fileNodes ?? new List<IFileNode>()).ToList().AsReadOnly();
        }

        public void Vist(IVistor vistor)
        {
            vistor.VistCommit(this);
        }
    }
}
