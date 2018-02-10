using System;

namespace TfsMigrate.Core.CommitTree.NodeTypes
{
    public interface INameNode : INode
    {
        string NodeName { get; }

        string Name { get; }

        string Email { get; }

        DateTimeOffset Date { get; }
    }
}

