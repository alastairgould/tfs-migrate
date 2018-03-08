using System;
using System.Threading;
using System.Threading.Tasks;

namespace TfsMigrate.Core.UseCases.PublishToVstsRepository
{
    public interface ICreateVstsGitRepository
    {
        Task<string> CreateRepository(Uri projectCollection, string teamProject, string gitRepository, CancellationToken cancellationToken);
    }
}
