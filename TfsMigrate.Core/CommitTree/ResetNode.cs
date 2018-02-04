using TfsMigrate.Core.GitFastImport;

namespace TfsMigrate.Core.CommitTree
{
    public class ResetNode : INode
    {
        public string Reference { get; private set; }
        public MarkReferenceNode From { get; private set; }

        public ResetNode(string reference, MarkReferenceNode from)
        {
            this.Reference = reference;
            this.From = from;
        }

        public void Vist(IVistor vistor)
        {
            vistor.VistReset(this);
        }
    }
}
