using FluentValidation;
using TfsMigrate.Contracts;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class ConvertTfsToGitCommandValidator : AbstractValidator<ConvertTfsToGitCommand>
    {
        public ConvertTfsToGitCommandValidator()
        {
            RuleFor(command => command.RepositoryDirectory).NotNull().NotEmpty();
            RuleFor(command => command.TfsRepositories).NotNull().NotEmpty();
            RuleFor(command => command.TfsRepositories).SetCollectionValidator(new TfsRepositoryValidator());
        }

        private class TfsRepositoryValidator : AbstractValidator<TfsRepository>
        {
            public TfsRepositoryValidator()
            {
                RuleFor(repo => repo.ProjectCollection).NotNull();
                RuleFor(repo => repo.Path).NotNull().NotEmpty();
            }
        }
    }
}
