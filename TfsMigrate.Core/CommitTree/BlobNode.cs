using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class BlobNode : IMarkNode
    {


        public bool IsRendered { get; set; }

        public DataNode DataNode { get; }

        public string Filename { get; private set; }

        public int? MarkId { get; }

        public bool HasBeenRendered { get; set; }

        public BlobNode(DataNode data, int? markId)
        {
            DataNode = data;
            MarkId = markId;
            IsRendered = false;
        }

        public BlobNode(Task<byte[]> data, int? markId)
            : this(new DataNode(data), markId) { }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistBlob(this);
        }
    }
}
