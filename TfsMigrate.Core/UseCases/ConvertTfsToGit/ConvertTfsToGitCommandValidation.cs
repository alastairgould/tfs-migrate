using FluentValidation;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class ConvertTfsToGitCommandValidation : AbstractValidator<ConvertTfsToGitCommand>
    {
        public ConvertTfsToGitCommandValidation()
        {
            RuleFor(command => command.Repositories).NotNull();
            RuleFor(command => command.RepositoryDirectory).NotNull();
        }
    }
}
