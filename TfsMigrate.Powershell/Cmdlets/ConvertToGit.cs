using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TfsMigrate.Contracts;
using TfsMigrate.Core.UseCases.ConvertTfsToGit;
using TfsMigrate.Core.UseCases.ConvertTfsToGit.Events;

namespace TfsMigrate.Powershell.Cmdlets
{
    [Cmdlet(VerbsData.ConvertTo, "Git")]
    public class ConvertToGit : Cmdlet, INotificationHandler<ProgressNotification>
    {
        [Parameter(
            Position = 0,
            ValueFromPipeline = true
        )]
        [ValidateNotNullOrEmpty]
        public TfsRepository[] Repositories { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true
        )]
        [ValidateNotNullOrEmpty]
        public string LocalRepositoryPath { get; set; }

        private IMediator _mediator; 

        protected override void BeginProcessing()
        {
            _mediator = SetupMediator.CreateMediator(this);
        }

        protected override void ProcessRecord()
        {
            var result = _mediator.Send(new ConvertTfsToGitCommand(Repositories,
                LocalRepositoryPath)).Result;

            WriteObject(result);
        }

        public Task Handle(ProgressNotification notification, CancellationToken cancellationToken)
        {
            var progress = new ProgressRecord(
                1, 
                "Convert Tfs Repository to Git",
                $"{notification.CurrentAmount} of {notification.AmountToProccess} Commits Processed");

            progress.PercentComplete = notification.PercentComplete;
            progress.CurrentOperation = $"Currently processing commit: {notification.CurrentCommit}";
            WriteProgress(progress);
            return Task.CompletedTask;
        }
    }
}
