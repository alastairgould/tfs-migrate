using System.Collections.Generic;
using System.Management.Automation;
using TfsMigrate.Contracts;

namespace TfsMigrate.Powershell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "TfsRepository")]
    public class GetTfsRepository : Cmdlet
    {
        [Parameter(Position = 0, ValueFromPipeline = true)]
        public TfsRepository[] Repositories { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string ProjectCollection { get; set; }

        [Parameter(Position = 2, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Path { get; set; }

        [Parameter(Position = 3)]
        [ValidateNotNullOrEmpty]
        public SwitchParameter ImportWorkItems { get; set; }

        [Parameter(Position = 4)]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Rename { get; set; }

        protected override void ProcessRecord()
        {
            var repos = new List<TfsRepository>();

            if(Repositories != null && Repositories.Length != 0)
                repos.AddRange(Repositories);

            repos.Add(new TfsRepository()
            {
                ProjectCollection = new System.Uri(ProjectCollection),
                Path = Path,
                ImportWorkItems = ImportWorkItems,
                RenamedTo = Rename
            });

            WriteObject(repos.ToArray());
        }
    }
}
