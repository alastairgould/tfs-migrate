using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TfsMigrate.Core.UseCases.ImportWorkItemAssociations.Events;

namespace TfsMigrate.Core.UseCases.ImportWorkItemAssociations
{
    public class ImportWorkItemAssociationsCommandHandler : IRequestHandler<ImportWorkItemAssociationsCommand>
    {
        private readonly IValidator<ImportWorkItemAssociationsCommand> _commandValidator;
        private readonly IMediator _mediator;
        private readonly ICreateWorkItemLink _createWorkItemLink;

        public ImportWorkItemAssociationsCommandHandler(IMediator mediator,
            ICreateWorkItemLink createWorkItemLink)
        { 
            _commandValidator = new ImportWorkItemAssociationsCommandValidator();
            _mediator = mediator;
            _createWorkItemLink = createWorkItemLink;
        }

        public Task Handle(ImportWorkItemAssociationsCommand command, CancellationToken cancellationToken)
        {
            _commandValidator.ValidateAndThrow(command);

            var projectCollection = command.VstsGitRepository.ProjectCollection;
            var teamProjectName = command.VstsGitRepository.TeamProject;
            var repositoryName = command.VstsGitRepository.RepositoryName;

            var total = command.VstsGitRepository.GitRepository.CommitWorkItemAssociations.Count;
            var processed = 0;

            foreach (var commitAssociations in command.VstsGitRepository.GitRepository.CommitWorkItemAssociations)
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

        public void UpdateProgress(string commitSha, int amountProccessed, int total)
        {
            _mediator.Publish(new ProgressNotification(
                amountProccessed,
                total,
                commitSha));
        }
    }
}