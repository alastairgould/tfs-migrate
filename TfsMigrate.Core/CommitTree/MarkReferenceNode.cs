using TfsMigrate.Core.GitFastImport;

namespace TfsMigrate.Core.CommitTree
{
    public class MarkReferenceNode : INode
    {
        public int? MarkId { get { return MarkNode.MarkId; } }
        public bool HasBeenRendered { get { return MarkNode.HasBeenRendered; } }
        public IMarkNode MarkNode { get; set;}

        public MarkReferenceNode(IMarkNode markNode)
        {
            MarkNode = markNode;
        }

        public void Vist(IVistor vistor)
        {
            vistor.VistMarkReference(this);
        }
    }
}
