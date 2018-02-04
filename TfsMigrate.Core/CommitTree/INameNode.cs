using System;

namespace TfsMigrate.Core.CommitTree
{
    public interface INameNode : INode
    {
        string CommandName { get; }
        string Name { get; }
        string Email { get; }
        DateTimeOffset Date { get; }
    }
}

