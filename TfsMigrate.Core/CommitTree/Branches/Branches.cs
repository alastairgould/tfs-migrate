using System.Collections.Generic;
using System.IO;
using System.Linq;
using TfsMigrate.Core.CommitTree.NodeTypes;

namespace TfsMigrate.Core.CommitTree.Branches
{
    public class Branches
    {
        public Branch CurrentBranch { get; private set; }

        private readonly List<Branch> _branches;

        public Branches()
        {
            _branches = new List<Branch>();
        }

        public Branch GetBranch(string serverPath) => _branches.FirstOrDefault(branch => serverPath.StartsWith(branch.TfsPath));

        public void CreateNewBranch(string serverPath, List<IFileNode> fileNodes)
        {
            // Assumes that main directory for branch is the first thing added in new branch
            var key = serverPath + "/";

            if (!_branches.Exists(branch => branch.TfsPath == key))
            {
                _branches.Add(new Branch(key, Path.GetFileName(serverPath), default(CommitNode)));
                fileNodes.Add(new FileDeleteAllNode());
            }

            ChangeCurrentBranch(_branches.Single(branch => branch.TfsPath == key));
        }

        public string GetPath(string serverPath, List<IFileNode> fileNodes)
        {
            if (CurrentBranch == null)
            {
                var branch = GetBranch(serverPath);

                if (branch == null)
                {
                    CreateNewBranch(serverPath, fileNodes);
                    return "";
                }
                
                ChangeCurrentBranch(branch);
            }

            if (!serverPath.StartsWith(CurrentBranch.TfsPath))
            {
                return null;
            }

            return serverPath.Replace(CurrentBranch.TfsPath, "");
        }

        private void ChangeCurrentBranch(Branch branch)
        {
            CurrentBranch = branch;
        }

        public void DeleteCurrentBranch()
        {
            _branches.Remove(CurrentBranch);
            CurrentBranch = null;
        }

        public void UpdateHeadCommitForCurrentBranch(CommitNode commit)
        {
            CurrentBranch.UpdateHead(commit);
        }
    }
}
