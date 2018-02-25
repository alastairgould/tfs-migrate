using System.Collections.Generic;
using MediatR;
using TfsMigrate.Contracts;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class ConvertTfsToGitCommand : IRequest<GitRepository>
    {
        public IEnumerable<TfsRepository> Repositories { get; }
        
        public string RepositoryDirectory { get; }

        public ConvertTfsToGitCommand(IEnumerable<TfsRepository> repositories, string outputDirectory)
        {
            Repositories = repositories;
            RepositoryDirectory = outputDirectory;
        }
    }
}
