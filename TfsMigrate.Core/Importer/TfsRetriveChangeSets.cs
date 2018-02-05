using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TfsMigrate.Core.Importer
{
    public class TfsRetriveChangeSets : IRetriveChangeSets
    {
        public IEnumerable<Changeset> RetriveChangeSets(Uri tfsProjectCollection, string tfsPath)
        {
            var collection = new TfsTeamProjectCollection(tfsProjectCollection);
            collection.EnsureAuthenticated();
            var versionControl = collection.GetService<VersionControlServer>();

            return versionControl
                .QueryHistory(
                    tfsPath,
                    VersionSpec.Latest,
                    0,
                    RecursionType.Full,
                    null,
                    new ChangesetVersionSpec(1),
                    VersionSpec.Latest,
                    int.MaxValue,
                    true,
                    false)
                .OfType<Changeset>()
                .OrderBy(x => x.ChangesetId);
        }
    }
}
