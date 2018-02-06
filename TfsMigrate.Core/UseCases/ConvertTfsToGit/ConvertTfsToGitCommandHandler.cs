using System;
using System.Collections.Generic;
using System.Linq;
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

            var branches = new Dictionary<string, Tuple<string, CommitNode>>();

            using (GitStreamWriter writer = new GitStreamWriter(convertTfsToGitCommand.RepositoryDirectory))
            {
                bool shouldSkipFirstCommit = false;

                foreach(var reposistory in convertTfsToGitCommand.Repositories)
                {
                    ConvertRepository(branches, writer, reposistory, shouldSkipFirstCommit);
                    shouldSkipFirstCommit = true;
                }
            }

            return Task.CompletedTask;
        }

        private void ConvertRepository(Dictionary<string, Tuple<string, CommitNode>> branches,
            GitStreamWriter writer,
            Contracts.TfsRepository reposistory,
            bool shouldSkipFirstCommit)
        {
            var changeSets = retriveChangeSets.RetriveChangeSets(reposistory.ProjectCollection, reposistory.Path);

            if(shouldSkipFirstCommit)
            {
                changeSets = changeSets.Skip(1);
            }

            foreach (var changeSet in changeSets)
            {
                ConvertChangeSet(branches, changeSet, writer);
            }
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
