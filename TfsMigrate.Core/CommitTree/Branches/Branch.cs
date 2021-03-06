﻿namespace TfsMigrate.Core.CommitTree.Branches
{
    public class Branch
    {
        public string GitBranchName => @"refs/heads/" + BranchName;

        public string BranchName { get; }

        public string TfsPath { get; private set; }

        public CommitNode Head { get; private set; }

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

        public void RenamePath(string path)
        {
            if(!path.EndsWith("/"))
            {
                path = path + "/";
            }

            TfsPath = path;
        }
    }
}
