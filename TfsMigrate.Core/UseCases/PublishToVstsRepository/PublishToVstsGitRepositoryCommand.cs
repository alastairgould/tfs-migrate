using System;
using MediatR;
using TfsMigrate.Contracts;

namespace TfsMigrate.Core.UseCases.PublishToVstsRepository
{
    public class PublishToVstsGitRepositoryCommand : IRequest
    {
        public Uri ProjectCollection { get; }

        public string TeamProject { get; }

        public string RepositoryName { get; }

        public GitRepository Repository { get; }

        public PublishToVstsGitRepositoryCommand(Uri projectCollection,
            string teamProject,
            string repositoryName, 
            GitRepository gitRepository)
        {
            ProjectCollection = projectCollection;
            TeamProject = teamProject;
            RepositoryName = repositoryName;
            Repository = gitRepository;
        }
    }
}
