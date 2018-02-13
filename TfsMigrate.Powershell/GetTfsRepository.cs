using System.Management.Automation;
using TfsMigrate.Contracts;

namespace TfsMigrate.Powershell
{
    [Cmdlet(VerbsCommon.Get, "TfsRepository")]
    public class GetTfsRepository : Cmdlet
    {
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string ProjectCollection { get; set; }

        [Parameter(Position = 1)]
        [ValidateNotNullOrEmpty]
        public string Path { get; set; }

        protected override void ProcessRecord()
        {
            WriteObject(new TfsRepository()
            {
                ProjectCollection = new System.Uri(ProjectCollection),
                Path = Path
            });
        }
    }
}
