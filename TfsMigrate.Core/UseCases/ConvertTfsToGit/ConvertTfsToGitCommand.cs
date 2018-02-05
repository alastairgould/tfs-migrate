using System;
using MediatR;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class ConvertTfsToGitCommand : IRequest
    {
        public Uri TfsProjectCollection { get; }

        public string TfsPath { get; }
        
        public string RepositoryDirectory { get; }

        public ConvertTfsToGitCommand(Uri tfsProjectCollection, string tfsPath, string outputDirectory)
        {
            TfsProjectCollection = tfsProjectCollection;
            TfsPath = tfsPath;
            RepositoryDirectory = outputDirectory;
        }
    }
}
