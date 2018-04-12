using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TfsMigrate.Contracts;
using TfsMigrate.Core.UseCases.ImportWorkItemAssociations;
using TfsMigrate.Core.UseCases.ImportWorkItemAssociations.Events;

namespace TfsMigrate.Powershell.Cmdlets
{
    [Cmdlet(VerbsData.Import, "WorkItemAssocations")]
    public class ImportWorkItemAssocations : PSCmdlet, INotificationHandler<ProgressNotification>
    {
        [Parameter(Position = 0, ValueFromPipeline = true, Mandatory = true, ParameterSetName = "FromPublish")]
        [ValidateNotNullOrEmpty]
        public VstsGitRepository VstsGitRepository { get; set; }

        [Parameter(Position = 0, ValueFromPipeline = true, Mandatory = true, ParameterSetName = "FromFile")]
        [ValidateNotNullOrEmpty]
        public PSObject CommitWorkItemAssociations { get; set; }

        [Parameter(Position = 0, ValueFromPipeline = false, Mandatory = true, ParameterSetName = "FromFile")]
        [ValidateNotNullOrEmpty]
        public Uri ProjectCollection { get; set; }

        [Parameter(Position = 0, ValueFromPipeline = false, Mandatory = true, ParameterSetName = "FromFile")]
        [ValidateNotNullOrEmpty]
        public string TeamProject { get; set; }

        [Parameter(Position = 0, ValueFromPipeline = false, Mandatory = true, ParameterSetName = "FromFile")]
        [ValidateNotNullOrEmpty]
        public string RepositoryName { get; set; }

        private IMediator _mediator;

        protected override void BeginProcessing()
        {
            AppDomain.CurrentDomain.AssemblyResolve += BindingRedirect;
            _mediator = SetupMediator.CreateMediator(this);
        }

        protected override void ProcessRecord()
        {
            switch(ParameterSetName)
            {
                case "FromPublish":
                    _mediator.Send(new ImportWorkItemAssociationsCommand(VstsGitRepository)).Wait();
                    break;

                case "FromFile":
                    var workItemAssocations = CommitWorkItemAssociations.Properties.ToDictionary(prop => prop.Name, prop => ConvertToIntEnumerable((Object[])prop.Value)); 

                    var vstsRepository = new VstsGitRepository()
                    {
                        GitRepository = new GitRepository
                        {
                            CommitWorkItemAssociations = workItemAssocations
                        },
                        ProjectCollection = ProjectCollection,
                        TeamProject = TeamProject,
                        RepositoryName = RepositoryName
                    };
                    _mediator.Send(new ImportWorkItemAssociationsCommand(vstsRepository)).Wait();
                    break;
            }
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

        private static IEnumerable<int> ConvertToIntEnumerable(Object[] workItemIds)
        {
            return workItemIds.Select(id => (int)id).AsEnumerable();
        }
    }
}
