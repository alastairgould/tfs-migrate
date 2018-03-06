using System;
using System.Linq;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Git.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TfsMigrate.Core.UseCases.ImportWorkItemAssociations
{
    public class CreateWorkItemLink : ICreateWorkItemLink
    {
        public void CreateLink(Uri projectCollection, string teamProjectName, string gitRepoName, int workItemId, string sha)
        {
            var collection = new TfsTeamProjectCollection(projectCollection);
            var workItemStore = collection.GetService<WorkItemStore>();
            var gitRepoService = collection.GetService<GitRepositoryService>();

            var gitRepoGuid = FindGitRepoId(teamProjectName, gitRepoName, gitRepoService);
            var teamProjectGuid = FindTeamProjectId(teamProjectName, workItemStore);

            CreateLink(workItemStore, workItemId, sha, teamProjectGuid, gitRepoGuid);
        }

        private void CreateLink(WorkItemStore workItemStore, int workItemId, string sha,
            Guid teamProjectGuid, Guid gitRepoGuid)
        {
            var workItem = workItemStore.GetWorkItem(workItemId);

            var link = $"vstfs:///git/commit/{teamProjectGuid}%2f{gitRepoGuid}%2f{sha}";

            ExternalLink changesetLink = new ExternalLink(
                workItemStore.RegisteredLinkTypes[ArtifactLinkIds.Commit],
                link);

            workItem.Links.Add(changesetLink);
            workItem.Save();
        }

        private static Guid FindTeamProjectId(string teamProjectName, WorkItemStore workItemStore)
        {
            var teamProject = workItemStore.Projects
                .OfType<Project>()
                .Single(proj => proj.Name == teamProjectName);
            var teamProjectGuid = teamProject.Guid;
            return teamProjectGuid;
        }

        private static Guid FindGitRepoId(string teamProjectName, string gitRepoName, GitRepositoryService gitRepoService)
        {
            var gitProjectRepoService = gitRepoService.QueryRepositories(teamProjectName);
            var gitRepo = gitProjectRepoService.Single(gr => gr.Name == gitRepoName);
            var gitRepoGuid = gitRepo.Id;
            return gitRepoGuid;
        }
    }
}
