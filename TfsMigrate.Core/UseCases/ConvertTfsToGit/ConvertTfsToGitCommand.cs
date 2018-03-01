using System.Collections.Generic;
using MediatR;
using TfsMigrate.Contracts;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class ConvertTfsToGitCommand : IRequest<GitRepository>
    {
        public IEnumerable<TfsRepository> TfsRepositories { get; }
        
        public string RepositoryDirectory { get; }

        public ConvertTfsToGitCommand(IEnumerable<TfsRepository> tfsRepositories, string outputDirectory)
        {
            TfsRepositories = tfsRepositories;
            RepositoryDirectory = outputDirectory;
        }
    }
}
