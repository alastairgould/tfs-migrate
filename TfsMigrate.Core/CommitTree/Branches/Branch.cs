namespace TfsMigrate.Core.CommitTree.Branches
{
    public class Branch
    {
        public string GitBranchName => @"refs/heads/" + BranchName;

        public string BranchName { get; set; }

        public string TfsPath { get; set; }

        public CommitNode Head { get; set; }

        public Branch(string tfsPath, string branchName, CommitNode head)
        {
            TfsPath = tfsPath;
            BranchName = branchName;
            Head = head;
        }

        public void UpdateHead(CommitNode newHead)
        {
            Head = newHead;
        }
    }
}
