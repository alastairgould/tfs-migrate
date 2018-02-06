using System;
using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class AuthorNode : INameNode
    {
        public string NodeName => "author";

        public string Name { get; }

        public string Email { get; }

        public DateTimeOffset Date { get; }

        public AuthorNode(string name, string email, DateTimeOffset date)
        {
            this.Name = name;
            this.Email = email;
            this.Date = date;
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistAuthor(this);
        }
    }
}
