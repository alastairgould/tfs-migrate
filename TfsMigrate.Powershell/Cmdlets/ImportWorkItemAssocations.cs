using System;
using System.Management.Automation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TfsMigrate.Contracts;
using TfsMigrate.Core.UseCases.ImportWorkItemAssociation;
using TfsMigrate.Core.UseCases.ImportWorkItemAssociation.Events;

namespace TfsMigrate.Powershell.Cmdlets
{
    [Cmdlet(VerbsData.Import, "WorkItemAssocations")]
    public class ImportWorkItemAssocations : Cmdlet, INotificationHandler<ProgressNotification>
    {
        [Parameter(Position = 0, ValueFromPipeline = true, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public VstsGitRepository VstsGitRepository { get; set; }

        private IMediator _mediator;

        protected override void BeginProcessing()
        {
            AppDomain.CurrentDomain.AssemblyResolve += BindingRedirect;
            _mediator = SetupMediator.CreateMediator(this);
        }

        protected override void ProcessRecord()
        {
            _mediator.Send(new ImportWorkItemAssociationsCommand(VstsGitRepository)).Wait();
        }

        public static Assembly BindingRedirect(object sender, ResolveEventArgs args)
        {
            // Powershell cmdlets don't let you use binding redirects in the traditional way...

            var name = new AssemblyName(args.Name);

            switch (name.Name)
            {
                case "Newtonsoft.Json":
                    return typeof(Newtonsoft.Json.JsonSerializer).Assembly;

                default:
                    return null;
            }
        }

        public Task Handle(ProgressNotification notification, CancellationToken cancellationToken)
        {
            var progress = new ProgressRecord(
                1,
                "Associating work items to commits",
                $"{notification.CurrentAmount} of {notification.AmountToProccess} Commits Processed");

            progress.PercentComplete = notification.PercentComplete;
            progress.CurrentOperation = $"Currently processing commit: {notification.CommitSha}";

            WriteProgress(progress);
            return Task.CompletedTask;
        }
    }
}
