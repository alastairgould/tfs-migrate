using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using TfsMigrate.Contracts;
using TfsMigrate.Core.Exporter;
using GitRepository = Microsoft.TeamFoundation.SourceControl.WebApi.GitRepository;

namespace TfsMigrate.Core.UseCases.PublishToVstsRepository
{
    public class PublishToVstsGitRepositoryCommandHandler : IRequestHandler<PublishToVstsGitRepositoryCommand, VstsGitRepository>
    {
        public async Task<VstsGitRepository> Handle(PublishToVstsGitRepositoryCommand message, CancellationToken cancellationToken)
        {
            var gitRepository = await CreateVstsGitRepository(message, cancellationToken);
            PushLocalRepositoryToVsts(message, gitRepository);

            return new VstsGitRepository()
            {
                GitRepository = message.Repository,
                ProjectCollection =  message.ProjectCollection,
                RepositoryName = message.RepositoryName,
                TeamProject = message.TeamProject
            };
        }

        private static void PushLocalRepositoryToVsts(PublishToVstsGitRepositoryCommand message, GitRepository gitRepository)
        {
            var gitClient = new GitClient(message.Repository.Path);
            gitClient.AddUpstreamRemote(gitRepository.RemoteUrl);
            gitClient.PushAllUpstreamRemote();
        }

        private static async Task<GitRepository> CreateVstsGitRepository(PublishToVstsGitRepositoryCommand message,
            CancellationToken cancellationToken)
        {
            var collection = new TfsTeamProjectCollection(message.ProjectCollection);
            collection.EnsureAuthenticated();
            var vstsGitApi = collection.GetClient<GitHttpClient>();

            var gitRepository = new GitRepository()
            {
                Name = message.RepositoryName
            };

            return await vstsGitApi.CreateRepositoryAsync(
                gitRepository,
                message.TeamProject,
                cancellationToken: cancellationToken);
        }
    }
}
