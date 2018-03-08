using System;
using TfsMigrate.Contracts;
using TfsMigrate.Core.UseCases.PublishToVstsRepository;

namespace TfsMigrate.Tests.UseCases.Builders
{
    public class PublishToVstsGitRepositoryCommandBuilder
    {
        public Uri ProjectCollection { get; set; }

        public string TeamProject { get; set; }

        public string RepositoryName { get; set; }

        public GitRepository Repository { get; set; }

        public PublishToVstsGitRepositoryCommandBuilder()
        {
            ProjectCollection = new Uri("http://www.test.com");
            TeamProject = "testProject";
            RepositoryName = "testRepository";
            Repository = new GitRepository()
            {
                Path = "testPath"
            };
        }

        public PublishToVstsGitRepositoryCommand CreateCommand()
        {
            return new PublishToVstsGitRepositoryCommand(ProjectCollection, TeamProject, RepositoryName, Repository);
        }
    }
}
