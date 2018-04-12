using System.Management.Automation;
using TfsMigrate.Contracts;

namespace TfsMigrate.Powershell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkItemAssociations")]
    public class GetWorkItemAssociations : Cmdlet
    {
        [Parameter(Position = 0, ValueFromPipeline = true, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public GitRepository GitRepository { get; set; }

        protected override void ProcessRecord()
        {
            WriteObject(GitRepository.CommitWorkItemAssociations);
        }
    }
}
