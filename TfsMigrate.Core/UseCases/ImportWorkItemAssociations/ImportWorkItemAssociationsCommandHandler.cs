using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Git.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsMigrate.Core.UseCases.ImportWorkItemAssociations.Events;

namespace TfsMigrate.Core.UseCases.ImportWorkItemAssociations
{
    public class ImportWorkItemAssociationsCommandHandler : IRequestHandler<ImportWorkItemAssociationsCommand>
    {
        private readonly IMediator _mediator;

        public ImportWorkItemAssociationsCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Handle(ImportWorkItemAssociationsCommand message, CancellationToken cancellationToken)
        {
            var collection = new TfsTeamProjectCollection(message.VstsGitRepository.ProjectCollection);
            var workItemStore = collection.GetService<WorkItemStore>();
            var gitRepoService = collection.GetService<GitRepositoryService>();
            var gitProjectRepoService = gitRepoService.QueryRepositories(message.VstsGitRepository.TeamProject);

            var teamProject = workItemStore.Projects
                .OfType<Project>()
                .Single(proj => proj.Name == message.VstsGitRepository.TeamProject);

            var gitRepo = gitProjectRepoService.Single(gr => gr.Name == message.VstsGitRepository.RepositoryName);

            var gitRepoGuid = gitRepo.Id;
            var teamProjectGuid = teamProject.Guid;

            var total = message.VstsGitRepository.GitRepository.CommitWorkItemAssociations.Count;
            var processed = 0;

            foreach (var commitAssoications in message.VstsGitRepository.GitRepository.CommitWorkItemAssociations)
            {
                UpdateProgress(commitAssoications.Key, processed, total);

                foreach (var workItemId in commitAssoications.Value)
                {
                    CreateLinks(workItemStore, workItemId, teamProjectGuid, gitRepoGuid, commitAssoications);
                }

                processed++;
            }

            return Task.CompletedTask;
        }

        private static void CreateLinks(WorkItemStore workItemStore, int workItemId, Guid teamProjectGuid, Guid gitRepoGuid,
            KeyValuePair<string, IEnumerable<int>> commitAssoications)
        {
            var workItem = workItemStore.GetWorkItem(workItemId);

            var link = $"vstfs:///git/commit/{teamProjectGuid}%2f{gitRepoGuid}%2f{commitAssoications.Key}";

            ExternalLink changesetLink = new ExternalLink(
                workItemStore.RegisteredLinkTypes[ArtifactLinkIds.Commit],
                link);

            workItem.Links.Add(changesetLink);
            workItem.Save();
        }

        public void UpdateProgress(string commitSha, int amountProccessed, int total)
        {
            _mediator.Publish(new ProgressNotification(
                amountProccessed,
                total,
                commitSha));
        }
    }
}