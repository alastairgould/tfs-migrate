using System.Collections.ObjectModel;
using System.Text;
using TfsMigrate.Core.GitFastImport;

namespace TfsMigrate.Core.CommitTree
{
    public class DataNode : INode
    {
        public ReadOnlyCollection<byte> Bytes { get { return new ReadOnlyCollection<byte>(this._Bytes); } }
        internal byte[] _Bytes;

        public DataNode(byte[] bytes)
        {
            this._Bytes = (byte[])bytes.Clone();
        }

        public DataNode(string str)
        {
            this._Bytes = Encoding.UTF8.GetBytes(str);
        }

        public void Vist(IVistor vistor)
        {
            vistor.VistData(this);
        }
    }
}
