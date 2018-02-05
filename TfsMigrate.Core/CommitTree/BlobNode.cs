using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
                for (int ix = 0; ix < x.Length; ++ix)
                    if (x[ix] != y[ix]) return false;
                return true;
            }
            public int GetHashCode(byte[] obj)
            {
                int retval = 0;
                foreach (byte value in obj) retval = (retval << 6) ^ value;
                return retval;
            }
        }

        private static Dictionary<byte[], BlobNode> _DataBlobs = new Dictionary<byte[], BlobNode>(new ByteComparer());

        public static BlobNode BuildBlob(byte[] data, int? markId)
        {
            var hasher = SHA1.Create();
            var hash = hasher.ComputeHash(data);
            if (_DataBlobs.ContainsKey(hash))
            {
                var blob = _DataBlobs[hash];
                if (blob.DataNode._Bytes.Length != data.Length)
                    throw new InvalidOperationException("There are two matching hashes, but the data are of two different lengths.");
                return blob;
            }
            else
            {
                var blob = new BlobNode(data, markId);
                _DataBlobs[hash] = blob;
                return blob;
            }
        }

        public bool IsRendered { get; set; }

        public DataNode DataNode { get; private set; }

        public string Filename { get; private set; }

        public int? MarkId { get; private set; }

        public bool HasBeenRendered { get; set; }

        private BlobNode(DataNode data, int? markId)
        {
            this.DataNode = data;
            this.MarkId = markId;
            this.IsRendered = false;
        }

        private BlobNode(byte[] data, int? markId)
            : this(new DataNode(data), markId) { }

        public void Vist(ITraverseCommitTree vistor)
        {
            vistor.VistBlob(this);
        }
    }
}
