using System;
using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class CommitterNode : INameNode
    {
        public string NodeName => "committer";

        public string Name { get; }

        public string Email { get; }

        public DateTimeOffset Date { get; }

        public CommitterNode(string name, string email, DateTimeOffset date)
        {
            Name = name;
            Email = email;
            Date = date;
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistCommitter(this);
        }
    }
}
