using FluentValidation;

namespace TfsMigrate.Core.UseCases.ImportWorkItemAssociations
{
    public class ImportWorkItemAssociationsCommandValidator : AbstractValidator<ImportWorkItemAssociationsCommand>
    {
        public ImportWorkItemAssociationsCommandValidator()
        {
            RuleFor(command => command.VstsGitRepository).NotNull();

            When(command => command.VstsGitRepository != null, () =>
            {
                RuleFor(command => command.VstsGitRepository.GitRepository).NotNull();
                RuleFor(command => command.VstsGitRepository.ProjectCollection).NotNull();
                RuleFor(command => command.VstsGitRepository.RepositoryName).NotNull().NotEmpty();
                RuleFor(command => command.VstsGitRepository.TeamProject).NotNull().NotEmpty();

                When(command => command.VstsGitRepository.GitRepository != null, () =>
                {
                    RuleFor(command => command.VstsGitRepository.GitRepository.CommitWorkItemAssociations).NotNull();
                });
            });
        }
    }
}
