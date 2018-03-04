using MediatR;
using TfsMigrate.Contracts;

namespace TfsMigrate.Core.UseCases.ImportWorkItemAssociations
{
    public class ImportWorkItemAssociationsCommand : IRequest
    {
        public VstsGitRepository VstsGitRepository { get; }

        public ImportWorkItemAssociationsCommand(VstsGitRepository vstsGitRepository)
        {
            VstsGitRepository = vstsGitRepository;
        }
    }
}
