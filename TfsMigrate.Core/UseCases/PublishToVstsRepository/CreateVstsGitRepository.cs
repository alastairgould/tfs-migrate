using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace TfsMigrate.Core.UseCases.PublishToVstsRepository
{
    public class CreateVstsGitRepository : ICreateVstsGitRepository
    {
        public async Task<string> CreateRepository(Uri projectCollection, string teamProject, string gitRepositoryName, CancellationToken cancellationToken)
        {
            var collection = new TfsTeamProjectCollection(projectCollection);
            collection.EnsureAuthenticated();
            var vstsGitApi = collection.GetClient<GitHttpClient>();

            var gitRepository = new GitRepository()
            {
                Name = gitRepositoryName
            };

            gitRepository = await vstsGitApi.CreateRepositoryAsync(
                gitRepository,
                teamProject,
                cancellationToken: cancellationToken);

            return gitRepository.RemoteUrl;
        }
    }
}
