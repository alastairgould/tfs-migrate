using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TfsMigrate.Core.Importer
{
    public class TfsRetriveChangeSets : IRetriveChangeSets
    {
        public IEnumerable<Changeset> RetriveChangeSets(Uri tfsProjectCollection, string tfsPath, bool startFrom = false)
        {
            var collection = new TfsTeamProjectCollection(tfsProjectCollection);
            collection.EnsureAuthenticated();
            var versionControl = collection.GetService<VersionControlServer>();

            ChangesetVersionSpec changeSetToStartFrom = null;

            if (startFrom)
            {
                changeSetToStartFrom = new ChangesetVersionSpec(211223);
            }
            else
            {
                changeSetToStartFrom = new ChangesetVersionSpec(1);
            }
            
            
            return versionControl
                .QueryHistory(
                    tfsPath,
                    VersionSpec.Latest,
                    0,
                    RecursionType.Full,
                    null,
                    changeSetToStartFrom,
                    VersionSpec.Latest,
                    int.MaxValue,
                    true,
                    false)
                .OfType<Changeset>()
                .OrderBy(x => x.ChangesetId);
        }
    }
}
