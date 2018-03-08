using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TfsMigrate.Core.UseCases.ImportWorkItemAssociations.Events;

namespace TfsMigrate.Core.UseCases.ImportWorkItemAssociations
{
    public class ImportWorkItemAssociationsCommandHandler : IRequestHandler<ImportWorkItemAssociationsCommand>
    {
        private readonly IMediator _mediator;
        private readonly ICreateWorkItemLink _createWorkItemLink;
        private readonly IValidator<ImportWorkItemAssociationsCommand> _commandValidator;

        public ImportWorkItemAssociationsCommandHandler(IMediator mediator,
            ICreateWorkItemLink createWorkItemLink)
        { 
            _mediator = mediator;
            _createWorkItemLink = createWorkItemLink;
            _commandValidator = new ImportWorkItemAssociationsCommandValidator();
        }

        public Task Handle(ImportWorkItemAssociationsCommand message, CancellationToken cancellationToken)
        {
            _commandValidator.ValidateAndThrow(message);

            var projectCollection = message.VstsGitRepository.ProjectCollection;
            var teamProjectName = message.VstsGitRepository.TeamProject;
            var repositoryName = message.VstsGitRepository.RepositoryName;

            var total = message.VstsGitRepository.GitRepository.CommitWorkItemAssociations.Count;
            var processed = 0;

            foreach (var commitAssociations in message.VstsGitRepository.GitRepository.CommitWorkItemAssociations)
            {
                UpdateProgress(commitAssociations.Key, processed, total);

                foreach (var workItemId in commitAssociations.Value)
                {
                    _createWorkItemLink.CreateLink(projectCollection, teamProjectName, repositoryName, workItemId, commitAssociations.Key);
                }

                processed++;
            }

            return Task.CompletedTask;
        }

        private void UpdateProgress(string commitSha, int amountProccessed, int total)
        {
            _mediator.Publish(new ProgressNotification(
                amountProccessed,
                total,
                commitSha));
        }
    }
}