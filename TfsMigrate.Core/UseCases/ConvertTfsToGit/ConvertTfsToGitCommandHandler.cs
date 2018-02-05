using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TfsMigrate.Core.CommitTree;
using TfsMigrate.Core.Exporter;
using TfsMigrate.Core.Importer;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class ConvertTfsToGitCommandHandler : IRequestHandler<ConvertTfsToGitCommand>
    {
        private readonly IRetriveChangeSets retriveChangeSets;

        public ConvertTfsToGitCommandHandler(IRetriveChangeSets retriveChangeSets)
        {
            this.retriveChangeSets = retriveChangeSets;
        }

        public Task Handle(ConvertTfsToGitCommand convertTfsToGitCommand, CancellationToken cancellationToken)
        {
            var validator = new ConvertTfsToGitCommandValidation();
            var validationResults = validator.Validate(convertTfsToGitCommand);

            var changeSets = retriveChangeSets.RetriveChangeSets(convertTfsToGitCommand.TfsProjectCollection, convertTfsToGitCommand.TfsPath);
            var branches = new Dictionary<string, Tuple<string, CommitNode>>();

            using (GitStreamWriter writer = new GitStreamWriter(convertTfsToGitCommand.RepositoryDirectory))
            {
                foreach (var changeSet in changeSets)
                {
                    ConvertChangeSet(branches, changeSet, writer);
                }
            }

            return Task.CompletedTask;
        }

        private void ConvertChangeSet(Dictionary<string, Tuple<string, CommitNode>> branches, 
            Microsoft.TeamFoundation.VersionControl.Client.Changeset changeSet,
            GitStreamWriter stream)
        {
            var commitTreeGenerator = new TfsCreateCommitTree();
            var commitTree = commitTreeGenerator.CreateCommitTree(changeSet, branches);

            var gitFastExport = new GitFastImport(stream);
            gitFastExport.ProccessCommit(commitTree);
        }
    }
}
