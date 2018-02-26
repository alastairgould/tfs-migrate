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

        [Parameter(Position = 1)]
        [ValidateNotNullOrEmpty]
        public string ProjectCollection { get; set; }

        [Parameter(Position = 2)]
        [ValidateNotNullOrEmpty]
        public string Path { get; set; }

        protected override void ProcessRecord()
        {
            var repos = new List<TfsRepository>();

            if(Repositories != null && Repositories.Length != 0)
                repos.AddRange(Repositories);

            repos.Add(new TfsRepository()
            {
                ProjectCollection = new System.Uri(ProjectCollection),
                Path = Path
            });

            WriteObject(repos.ToArray());
        }
    }
}
