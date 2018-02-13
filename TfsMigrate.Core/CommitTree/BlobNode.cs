using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TfsMigrate.Core.CommitTree.NodeTypes;
using TfsMigrate.Core.CommitTree.Traverse;

namespace TfsMigrate.Core.CommitTree
{
    public class BlobNode : IMarkNode
    {
        private class ByteComparer : IEqualityComparer<byte[]>
        {
            public bool Equals(byte[] x, byte[] y)
            {
                if (x.Length != y.Length) return false;
                for (var ix = 0; ix < x.Length; ++ix)
                    if (x[ix] != y[ix]) return false;
                return true;
            }
            public int GetHashCode(byte[] obj)
            {
                var retval = 0;
                foreach (var value in obj) retval = (retval << 6) ^ value;
                return retval;
            }
        }

        private static readonly Dictionary<byte[], BlobNode> _dataBlobs = new Dictionary<byte[], BlobNode>(new ByteComparer());

        public static BlobNode BuildBlob(byte[] data, int? markId)
        {
            var hasher = SHA1.Create();
            var hash = hasher.ComputeHash(data);
            if (_dataBlobs.ContainsKey(hash))
            {
                var blob = _dataBlobs[hash];
                if (blob.DataNode.Bytes.Length != data.Length)
                    throw new InvalidOperationException("There are two matching hashes, but the data are of two different lengths.");
                return blob;
            }
            else
            {
                var blob = new BlobNode(data, markId);
                _dataBlobs[hash] = blob;
                return blob;
            }
        }

        public bool IsRendered { get; set; }

        public DataNode DataNode { get; }

        public string Filename { get; private set; }

        public int? MarkId { get; }

        public bool HasBeenRendered { get; set; }

        private BlobNode(DataNode data, int? markId)
        {
            DataNode = data;
            MarkId = markId;
            IsRendered = false;
        }

        private BlobNode(byte[] data, int? markId)
            : this(new DataNode(data), markId) { }

        public void AcceptVisitor(ITraverseCommitTree vistor)
        {
            vistor.VistBlob(this);
        }
    }
}
