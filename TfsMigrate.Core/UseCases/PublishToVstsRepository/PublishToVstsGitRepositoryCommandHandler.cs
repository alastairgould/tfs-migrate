using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using TfsMigrate.Core.Exporter;

namespace TfsMigrate.Core.UseCases.PublishToVstsRepository
{
    public class PublishToVstsGitRepositoryCommandHandler : IRequestHandler<PublishToVstsGitRepositoryCommand>
    {
        public async Task Handle(PublishToVstsGitRepositoryCommand message, CancellationToken cancellationToken)
        {
            var gitRepository = await CreateVstsGitRepository(message, cancellationToken);
            PushLocalRepositoryToVsts(message, gitRepository);
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

            gitRepository = await vstsGitApi.CreateRepositoryAsync(
                gitRepository,
                message.TeamProject,
                cancellationToken: cancellationToken);
            return gitRepository;
        }
    }
}
