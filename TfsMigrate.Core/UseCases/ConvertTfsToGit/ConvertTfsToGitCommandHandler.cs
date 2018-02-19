using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.TeamFoundation.VersionControl.Client;
using TfsMigrate.Contracts;
using TfsMigrate.Core.CommitTree;
using TfsMigrate.Core.CommitTree.Branches;
using TfsMigrate.Core.Exporter;
using TfsMigrate.Core.Importer;
using TfsMigrate.Core.UseCases.ConvertTfsToGit.Events;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class ConvertTfsToGitCommandHandler : IRequestHandler<ConvertTfsToGitCommand>
    {
        private readonly IRetriveChangeSets _retriveChangeSets;
        private readonly IMediator _mediator;

        public ConvertTfsToGitCommandHandler(IRetriveChangeSets retriveChangeSets, IMediator mediator)
        {
            _retriveChangeSets = retriveChangeSets;
            _mediator = mediator;
        }

        public Task Handle(ConvertTfsToGitCommand convertTfsToGitCommand, CancellationToken cancellationToken)
        {
            var validator = new ConvertTfsToGitCommandValidation();
            var validationResults = validator.Validate(convertTfsToGitCommand);

            var branches = new Branches(); 

            using (var writer = GitStreamWriter.CreateGitStreamWriter(convertTfsToGitCommand.RepositoryDirectory))
            {
                var shouldSkipFirstCommit = false;

                foreach (var reposistory in convertTfsToGitCommand.Repositories)
                {
                    ConvertRepository(branches, writer, reposistory, shouldSkipFirstCommit);
                    shouldSkipFirstCommit = true;
                }
            }

            return Task.CompletedTask;
        }

        private void ConvertRepository(Branches branches,
            GitStreamWriter writer,
            TfsRepository reposistory,
            bool shouldSkipFirstCommit)
        {
            var changeSets = _retriveChangeSets.RetriveChangeSets(reposistory.ProjectCollection, reposistory.Path);
            var amountToProccess = changeSets.Count();

            if(shouldSkipFirstCommit)
            {
                changeSets = changeSets.Skip(1);
            }

            foreach (var changeSetAndIndex in changeSets.Select((value, i) => new { CurrentIndex = i, ChangeSet = value }))
            {
                ProgressNotification(changeSetAndIndex.CurrentIndex, amountToProccess, changeSetAndIndex.ChangeSet);
                ConvertChangeSet(branches, changeSetAndIndex.ChangeSet, writer);
            }
        }

        private void ConvertChangeSet(Branches branches,
            Changeset changeSet,
            GitStreamWriter stream)
        {
            var commitTreeGenerator = new TfsCreateCommitTree();
            var commitTree = commitTreeGenerator.CreateCommitTree(changeSet, branches);

            var gitFastExport = new GitFastImport(stream);
            gitFastExport.ProccessCommit(commitTree);
        }

        private void ProgressNotification(int currentAmount, int amountToProccess, Changeset changeSet)
        {
            var currentCommit = new CurrentCommit(changeSet.ChangesetId, changeSet.Comment);

            _mediator.Publish(new ProgressNotification(
                currentAmount,
                amountToProccess,
                currentCommit));
        }
    }
}
