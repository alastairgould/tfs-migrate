using System;
using System.Collections.Generic;
using System.Linq;
using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class CommitNode : IMarkNode
    {
        public string Reference { get; }

        public AuthorNode Author { get; }

        public CommitterNode Committer { get; }

        public DataNode CommitInfo { get; }

        public MarkReferenceNode<CommitNode> FromCommit { get; }

        public IList<MarkReferenceNode<CommitNode>> MergeCommits { get; }

        public IList<IFileNode> FileNodes { get; }

        public int? MarkId { get; }

        public bool HasBeenRendered { get; set; }

        public CommitNode(
            int markId,
            string reference,
            AuthorNode author,
            CommitterNode committer,
            DataNode commitInfo,
            MarkReferenceNode<CommitNode> fromCommit,
            IList<MarkReferenceNode<CommitNode>> mergeCommits,
            IList<IFileNode> fileNodes)
        {
            if (string.IsNullOrEmpty(reference))
                throw new InvalidOperationException("The Reference for this commit must be valid.");
            if (committer == null)
                throw new InvalidOperationException("A committer must be specified for this commit.");
            if (commitInfo == null)
                throw new InvalidOperationException("Commit Information must be specified for this commit.");

            MarkId = markId;
            Reference = reference;
            Author = author;
            Committer = committer;
            CommitInfo = commitInfo;
            FromCommit = fromCommit;
            MergeCommits = (mergeCommits ?? new List<MarkReferenceNode<CommitNode>>()).ToList().AsReadOnly();
            FileNodes = (fileNodes ?? new List<IFileNode>()).ToList().AsReadOnly();
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistCommit(this);
        }
    }
}
