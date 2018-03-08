using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TfsMigrate.Contracts;
using TfsMigrate.Core.Exporter;

namespace TfsMigrate.Core.UseCases.PublishToVstsRepository
{
    public class PublishToVstsGitRepositoryCommandHandler : IRequestHandler<PublishToVstsGitRepositoryCommand, VstsGitRepository>
    {
        private readonly IValidator<PublishToVstsGitRepositoryCommand> _commandValidator;
        private readonly ICreateVstsGitRepository _createVstsGitRepository;
        private readonly Func<string, IGitClient> _createGitClient;

        public PublishToVstsGitRepositoryCommandHandler(ICreateVstsGitRepository createVstsGitRepository,
            Func<string, IGitClient> createGitClient)
        {
            _commandValidator =  new PublishToVstsGitRepositoryCommandValidator();
            _createVstsGitRepository = createVstsGitRepository;
            _createGitClient = createGitClient;
        }

        public async Task<VstsGitRepository> Handle(PublishToVstsGitRepositoryCommand command, CancellationToken cancellationToken)
        {
            _commandValidator.ValidateAndThrow(command);

            var gitRepository = await _createVstsGitRepository.CreateRepository(command.ProjectCollection,
                command.TeamProject, command.RepositoryName, cancellationToken);

            PushLocalRepositoryToVsts(command, gitRepository);

            return new VstsGitRepository()
            {
                GitRepository = command.Repository,
                ProjectCollection =  command.ProjectCollection,
                RepositoryName = command.RepositoryName,
                TeamProject = command.TeamProject
            };
        }

        private void PushLocalRepositoryToVsts(PublishToVstsGitRepositoryCommand message, string gitRepositoryUrl)
        {
            var gitClient = _createGitClient(message.Repository.Path);
            gitClient.AddUpstreamRemote(gitRepositoryUrl);
            gitClient.PushAllUpstreamRemote();
        }
    }
}
