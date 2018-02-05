using System;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class CommitterNode : INameNode
    {
        public string NodeName => "committer";

        public string Name { get; private set; }

        public string Email { get; private set; }

        public DateTimeOffset Date { get; private set; }

        public CommitterNode(string name, string email, DateTimeOffset date)
        {
            this.Name = name;
            this.Email = email;
            this.Date = date;
        }

        public void Vist(ITraverseCommitTree vistor)
        {
            vistor.VistCommitter(this);
        }
    }
}
