using FluentValidation;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class ConvertTfsToGitCommandValidation : AbstractValidator<ConvertTfsToGitCommand>
    {
        public ConvertTfsToGitCommandValidation()
        {
            RuleFor(command => command.TfsProjectCollection).NotNull();
            RuleFor(command => command.TfsPath).NotNull();
            RuleFor(command => command.TfsPath).NotNull();
        }
    }
}
