using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.VersionControl.Client;
using Polly;
using TfsMigrate.Core.CommitTree;
using TfsMigrate.Core.CommitTree.NodeTypes;

namespace TfsMigrate.Core.Importer
{
    public class TfsCreateCommitTree
    {
        private List<IFileNode> fileNodes = new List<IFileNode>();

        private List<CommitNode> merges = new List<CommitNode>();

        private string branch = null;

        // 10,000,000 to get it out of way of normal checkins
        private static int _MarkID = 10000001;

        private BlobNode GetDataBlob(Item item)
        {
            var policy = Policy
                .Handle<VersionControlException>()
                .Or<TeamFoundationServerUnauthorizedException>()
                .Retry(5);

            return policy.Execute(() =>
            {
                var bytes = new byte[item.ContentLength];
                var str = item.DownloadFile();
                str.Read(bytes, 0, bytes.Length);
                str.Close();

                var id = _MarkID++;
                var blob = BlobNode.BuildBlob(bytes, id);
                return blob;
            });
        }

        private static BranchHistoryTreeItem FindMergedItem(BranchHistoryTreeItem parent, int changeSetId)
        {
            foreach (BranchHistoryTreeItem item in parent.Children)
            {
                if (item.Relative.IsRequestedItem)
                    return item;

                var x = FindMergedItem(item, changeSetId);
                if (x != null)
                    return x;
            }
            return null;
        }

        private Tuple<string, Tuple<string, CommitNode>> GetBranch(string serverPath, Dictionary<string, Tuple<string, CommitNode>> branches)
        {
            foreach (var x in branches)
                if (serverPath.StartsWith(x.Key))
                    return Tuple.Create(x.Key, x.Value);
            return null;
        }

        private string GetPath(string serverPath, Dictionary<string, Tuple<string, CommitNode>> branches)
        {
            if (branch == null)
            {
                var branchInfo = GetBranch(serverPath, branches);
                if (branchInfo == null)
                {
                    CreateNewBranch(serverPath, branches);
                    return "";
                }
                else
                    branch = branchInfo.Item1;
            }

            if (!serverPath.StartsWith(branch))
                // for now ignore secondary branches and hope that other filemodify Nodes work this stuff out
                return null;

            return serverPath.Replace(branch, "");
        }

        private void CreateNewBranch(string serverPath, Dictionary<string, Tuple<string, CommitNode>> branches)
        {
            // Assumes that main directory for branch is the first thing added in new branch
            branch = serverPath + "/";

            if (!branches.ContainsKey(branch))
            {
                branches[branch] = Tuple.Create($"refs/heads/{Path.GetFileName(serverPath)}", default(CommitNode));
                fileNodes.Add(new FileDeleteAllNode());
            }
        }

        #region Active Directory
        private static string ProcessADName(string adName)
        {
            if (string.IsNullOrEmpty(adName))
                return "";

            if (!adName.Contains('\\'))
                return adName;


            var split = adName.Split('\\');
            return split[1];
        }

        private static UserPrincipal GetUserPrincipal(string userName)
        {
            return null;
        }

        private static string GetEmailAddressForUser(string userName)
        {
            try
            {
                return GetUserPrincipal(userName).EmailAddress;
            }
            catch
            {
                return "no.user@example.com";
            }
        }

        #endregion
        public CommitNode CreateCommitTree(Changeset changeSet,
            Dictionary<string, Tuple<string, CommitNode>> branches)
        {
            var committer = new CommitterNode(changeSet.Committer, GetEmailAddressForUser(changeSet.Committer), changeSet.CreationDate);
            var author = changeSet.Committer != changeSet.Owner ? new AuthorNode(changeSet.Owner, GetEmailAddressForUser(changeSet.Owner), changeSet.CreationDate) : null;

            var orderedChanges = changeSet.Changes
                .Select((x, i) => new { x, i })
                .OrderBy(z => z.x.ChangeType)
                .ThenBy(z => z.i)
                .Select(z => z.x)
                .ToList();
            var deleteBranch = false;
            foreach (var change in orderedChanges)
            {
                var path = GetPath(change.Item.ServerItem, branches);
                if (path == null)
                    continue;

                // we delete before we check folders in case we can delete
                // an entire subdir w/ one Node instead of file by file
                if ((change.ChangeType & ChangeType.Delete) == ChangeType.Delete)
                {
                    fileNodes.Add(new FileDeleteNode(path));
                    if (path == "")
                    {
                        deleteBranch = true;
                        break;
                    }
                    continue;
                }

                if (change.Item.ItemType == ItemType.Folder)
                    continue;

                if ((change.ChangeType & ChangeType.Rename) == ChangeType.Rename)
                {
                    var vcs = change.Item.VersionControlServer;
                    var history = vcs
                        .QueryHistory(
                            change.Item.ServerItem,
                            new ChangesetVersionSpec(changeSet.ChangesetId),
                            change.Item.DeletionId,
                            RecursionType.None,
                            null,
                            null,
                            new ChangesetVersionSpec(changeSet.ChangesetId),
                            int.MaxValue,
                            true,
                            false)
                        .OfType<Changeset>()
                        .ToList();


                    var previousChangeset = history[1];
                    var previousFile = previousChangeset.Changes[0];
                    var previousPath = GetPath(previousFile.Item.ServerItem, branches);
                    fileNodes.Add(new FileRenameNode(previousPath, path));

                    // remove delete Nodes, since rename will take care of biz
                    fileNodes.RemoveAll(fc => fc is FileDeleteNode && fc.Path == previousPath);
                }

                var blob = GetDataBlob(change.Item);
                fileNodes.Add(new FileModifyNode(path, new MarkReferenceNode<BlobNode>(blob)));

                if ((change.ChangeType & ChangeType.Branch) == ChangeType.Branch)
                {
                    var vcs = change.Item.VersionControlServer;
                    var history = vcs.GetBranchHistory(new[] { new ItemSpec(change.Item.ServerItem, RecursionType.None) }, new ChangesetVersionSpec(changeSet.ChangesetId));

                    var itemHistory = history[0][0];
                    var mergedItem = FindMergedItem(itemHistory, changeSet.ChangesetId);
                    var branchInfo = GetBranch(mergedItem.Relative.BranchFromItem.ServerItem, branches).Item2;
                    var previousCommit = branchInfo.Item2;
                    if (!merges.Contains(previousCommit))
                        merges.Add(previousCommit);
                }

                if ((change.ChangeType & ChangeType.Merge) == ChangeType.Merge)
                {
                    var vcs = change.Item.VersionControlServer;
                    var mergeHistory = vcs.QueryMergesExtended(new ItemSpec(change.Item.ServerItem, RecursionType.None), new ChangesetVersionSpec(changeSet.ChangesetId), null, new ChangesetVersionSpec(changeSet.ChangesetId)).ToList();
                    foreach (var mh in mergeHistory)
                    {
                        var branchInfo = GetBranch(mh.SourceItem.Item.ServerItem, branches).Item2;
                        var previousCommit = branchInfo.Item2;
                        if (!merges.Contains(previousCommit))
                            merges.Add(previousCommit);
                    }
                }
            }

            var reference = branches[branch];
            var commit = new CommitNode(
                markId: changeSet.ChangesetId,
                reference: reference.Item1,
                committer: committer,
                author: author,
                commitInfo: new DataNode(changeSet.Comment),
                fromCommit: reference.Item2 != null ? new MarkReferenceNode<CommitNode>(reference.Item2) : null,
                mergeCommits: merges.Select(mergeCommit => new MarkReferenceNode<CommitNode>(mergeCommit)).ToList(),
                fileNodes: fileNodes);

            if (deleteBranch)
                branches.Remove(branch);
            else
                branches[branch] = Tuple.Create(reference.Item1, commit);

            return commit;
        }
    }
}
