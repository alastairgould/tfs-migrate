﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using TfsMigrate.Contracts;
using TfsMigrate.Core.Exporter;
using TfsMigrate.Core.Importer;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class ConvertTfsToGitCommandHandler : IRequestHandler<ConvertTfsToGitCommand, GitRepository>
    {
        private readonly IRetriveChangeSets _retriveChangeSets;
        private readonly IMediator _mediator;
        private readonly VersionControlState _versionControlState;

        private ChangeSetProgressNotifier _progressNotifier;

        public ConvertTfsToGitCommandHandler(IRetriveChangeSets retriveChangeSets,
            IMediator mediator)
        {
            _retriveChangeSets = retriveChangeSets;
            _mediator = mediator;
            _versionControlState = new VersionControlState();
        }

        public Task<GitRepository> Handle(ConvertTfsToGitCommand convertTfsToGitCommand, CancellationToken cancellationToken)
        {
            _progressNotifier = new ChangeSetProgressNotifier(convertTfsToGitCommand.TfsRepositories,
                _retriveChangeSets, _mediator);

            using (var writer = GitStreamWriter.CreateGitStreamWriter(convertTfsToGitCommand.RepositoryDirectory))
            {
                var shouldSkipFirstCommit = false;

                foreach (var tfsRepository in convertTfsToGitCommand.TfsRepositories)
                {
                    ConvertRepository(writer, tfsRepository, shouldSkipFirstCommit, tfsRepository.RenamedFrom);
                    shouldSkipFirstCommit = true;
                }
            }

            return Task.FromResult(new GitRepository()
            {
                Path = convertTfsToGitCommand.RepositoryDirectory,
                CommitWorkItemAssociations = _versionControlState.CommitWorkItemAssociations
            });
        }

        private void ConvertRepository(GitStreamWriter writer,
            TfsRepository tfsReposistory,
            bool shouldSkipFirstCommit,
            bool shouldSkipLastCommit)
        {
            var changeSets = _retriveChangeSets.RetriveChangeSets(tfsReposistory.ProjectCollection, tfsReposistory.Path);

            _versionControlState.Branches.CurrentBranch?.RenamePath(tfsReposistory.Path);

            if (shouldSkipFirstCommit)
            {
                changeSets = changeSets.Skip(1);
            }

            if (shouldSkipLastCommit)
            {
                changeSets = changeSets.Take(changeSets.Count() - 1);
            }

            foreach (var changeSet in changeSets)
            {
                _progressNotifier.NextChangeSet(changeSet);
                ConvertChangeSet(changeSet, writer, tfsReposistory.ImportWorkItems);
            }
        }

        private void ConvertChangeSet(Changeset changeSet,
            GitStreamWriter stream,
            bool saveWorkItemAssociations)
        {
            var commitTreeGenerator = new TfsCreateCommitTree();
            var commitTree = commitTreeGenerator.CreateCommitTree(changeSet, _versionControlState.Branches);

            var gitFastExport = new GitFastImport(stream);
            var commitId = gitFastExport.ProccessCommit(commitTree);

            if (saveWorkItemAssociations)
            {
                SaveAssociatedWorkItems(changeSet, commitId);
            }
        }

        private void SaveAssociatedWorkItems(Changeset changeSet, string commitId)
        {
            var workItems = changeSet.AssociatedWorkItems.Select(workItem => workItem.Id).ToList();

            if (!workItems.IsNullOrEmpty())
            {
                _versionControlState.CommitWorkItemAssociations[commitId] = workItems;
            }
        }
    }
}
