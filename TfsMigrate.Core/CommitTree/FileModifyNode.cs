﻿using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class FileModifyNode : IFileNode
    {
        public string Path { get; }

        public MarkReferenceNode<BlobNode> Blob { get; }

        public DataNode Data { get; }

        public FileModifyNode(string path, MarkReferenceNode<BlobNode> blob)
        {
            this.Path = path;
            this.Blob = blob;
        }

        public FileModifyNode(string path, byte[] data)
        {
            this.Path = path;
            this.Data = new DataNode(data);
        }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistFileModify(this);
        }
    }
}
