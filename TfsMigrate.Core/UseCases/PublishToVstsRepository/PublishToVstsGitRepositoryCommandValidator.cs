using FluentValidation;

namespace TfsMigrate.Core.UseCases.PublishToVstsRepository
{
    public class PublishToVstsGitRepositoryCommandValidator : AbstractValidator<PublishToVstsGitRepositoryCommand>
    {
        public PublishToVstsGitRepositoryCommandValidator()
        {
            RuleFor(command => command.ProjectCollection).NotNull();
            RuleFor(command => command.Repository).NotNull();
            RuleFor(command => command.RepositoryName).NotNull().NotEmpty();
            RuleFor(command => command.TeamProject).NotNull().NotEmpty();

            When(command => command.Repository != null, () =>
            {
                RuleFor(command => command.Repository.Path).NotNull().NotEmpty();
            });
        }
    }
}
