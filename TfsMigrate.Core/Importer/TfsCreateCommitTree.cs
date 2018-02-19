using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.VersionControl.Client;
using Polly;
using TfsMigrate.Core.CommitTree;
using TfsMigrate.Core.CommitTree.Branches;
using TfsMigrate.Core.CommitTree.NodeTypes;

namespace TfsMigrate.Core.Importer
{
    public class TfsCreateCommitTree
    {
        private readonly List<IFileNode> _fileNodes = new List<IFileNode>();

        private readonly List<CommitNode> _merges = new List<CommitNode>();

        // 10,000,000 to get it out of way of normal checkins
        private static int _markId = 10000001;

        private BlobNode GetDataBlob(Item item)
        {
            var policy = Policy
                .Handle<VersionControlException>()
                .Or<TeamFoundationServerUnauthorizedException>()
                .Retry(5);

            var task = Task.Factory.StartNew(() =>
            {
                return policy.Execute(() =>
                {
                    var bytes = new byte[item.ContentLength];
                    var str = item.DownloadFile();
                    str.Read(bytes, 0, bytes.Length);
                    str.Close();
                    return bytes;
                });
            });

            var id = _markId++;
            var blob = new BlobNode(task, id);
            return blob;
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

        #region Active Directory
        private static string ProcessAdName(string adName)
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
           Branches branches)
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
                var path = branches.GetPath(change.Item.ServerItem, _fileNodes);
                if (path == null)
                    continue;

                // we delete before we check folders in case we can delete
                // an entire subdir w/ one Node instead of file by file
                if ((change.ChangeType & ChangeType.Delete) == ChangeType.Delete)
                {
                    _fileNodes.Add(new FileDeleteNode(path));
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
                    var previousPath = branches.GetPath(previousFile.Item.ServerItem, _fileNodes);
                    _fileNodes.Add(new FileRenameNode(previousPath, path));

                    // remove delete Nodes, since rename will take care of biz
                    _fileNodes.RemoveAll(fc => fc is FileDeleteNode && fc.Path == previousPath);
                }

                var blob = GetDataBlob(change.Item);
                _fileNodes.Add(new FileModifyNode(path, new MarkReferenceNode<BlobNode>(blob)));

                if ((change.ChangeType & ChangeType.Branch) == ChangeType.Branch)
                {
                    var vcs = change.Item.VersionControlServer;
                    var history = vcs.GetBranchHistory(new[] { new ItemSpec(change.Item.ServerItem, RecursionType.None) }, new ChangesetVersionSpec(changeSet.ChangesetId));

                    var itemHistory = history[0][0];
                    var mergedItem = FindMergedItem(itemHistory, changeSet.ChangesetId);

                    var previousCommit = branches.GetBranch(mergedItem.Relative.BranchFromItem.ServerItem).Head;

                    if (!_merges.Contains(previousCommit))
                    {
                        _merges.Add(previousCommit);
                    }
                }

                if ((change.ChangeType & ChangeType.Merge) == ChangeType.Merge)
                {
                    var vcs = change.Item.VersionControlServer;
                    var mergeHistory = vcs.QueryMergesExtended(new ItemSpec(change.Item.ServerItem, RecursionType.None), new ChangesetVersionSpec(changeSet.ChangesetId), null, new ChangesetVersionSpec(changeSet.ChangesetId)).ToList();

                    foreach (var mh in mergeHistory)
                    {
                        var previousCommit = branches.GetBranch(mh.SourceItem.Item.ServerItem).Head;

                        if (!_merges.Contains(previousCommit))
                        {
                            _merges.Add(previousCommit);
                        }
                    }
                }
            }

            var reference = branches.CurrentBranch;

            var commit = new CommitNode(
                markId: changeSet.ChangesetId,
                reference: reference.GitBranchName,
                committer: committer,
                author: author,
                commitInfo: new DataNode(changeSet.Comment),
                fromCommit: reference.Head != null ? new MarkReferenceNode<CommitNode>(reference.Head) : null,
                mergeCommits: _merges.Select(mergeCommit => new MarkReferenceNode<CommitNode>(mergeCommit)).ToList(),
                fileNodes: _fileNodes);

            if (deleteBranch)
                branches.DeleteCurrentBranch();
            else
                branches.UpdateHeadCommitForCurrentBranch(commit);

            return commit;
        }
    }
}
