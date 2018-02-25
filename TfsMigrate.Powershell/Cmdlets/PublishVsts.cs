using System;
using System.Management.Automation;
using System.Reflection;
using MediatR;
using TfsMigrate.Contracts;
using TfsMigrate.Core.UseCases.PublishToVstsRepository;

namespace TfsMigrate.Powershell.Cmdlets
{
    [Cmdlet(VerbsData.Publish, "Vsts")]
    public class PublishVsts : Cmdlet
    {
        [Parameter(Position = 0, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public GitRepository GitRepository { get; set; }

        [Parameter(Position = 1)]
        [ValidateNotNullOrEmpty]
        public string ProjectCollection { get; set; }

        [Parameter(Position = 2)]
        [ValidateNotNullOrEmpty]
        public string TeamProject { get; set; }

        [Parameter(Position = 3)]
        [ValidateNotNullOrEmpty]
        public string RepositoryName { get; set; }

        private IMediator _mediator;

        protected override void BeginProcessing()
        {
            AppDomain.CurrentDomain.AssemblyResolve += BindingRedirect;
            _mediator = SetupMediator.CreateMediator();
        }

        protected override void ProcessRecord()
        {
            _mediator.Send(new PublishToVstsGitRepositoryCommand(new Uri(ProjectCollection), TeamProject, RepositoryName,
                GitRepository)).Wait();
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
    }
}
