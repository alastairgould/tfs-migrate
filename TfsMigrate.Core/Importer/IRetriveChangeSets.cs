using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TfsMigrate.Core.Importer
{
    public interface IRetriveChangeSets
    {
        IEnumerable<Changeset> RetriveChangeSets(Uri tfsProjectCollection, string tfsPath, bool startFrom);
    }
}
