using System.Collections.Generic;
using System.Linq;
using MediatR;
using Microsoft.TeamFoundation.VersionControl.Client;
using TfsMigrate.Contracts;
using TfsMigrate.Core.Importer;
using TfsMigrate.Core.UseCases.ConvertTfsToGit.Events;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class ChangeSetProgressNotifier
    {
        private readonly IRetriveChangeSets _retriveChangeSets;

        private readonly IEnumerable<TfsRepository> _repositories;

        private readonly IMediator _mediator;

        private readonly int _total;

        private int _current;

        public ChangeSetProgressNotifier(IEnumerable<TfsRepository> repositories,
            IRetriveChangeSets retriveChangeSets,
            IMediator mediator)
        {
            _retriveChangeSets = retriveChangeSets;
            _repositories = repositories;
            _total = CalculateTotal();
            _mediator = mediator;
        }

        private int CalculateTotal()
        {
            var sum = _repositories
                .Select(repo => _retriveChangeSets.RetriveChangeSets(repo.ProjectCollection, repo.Path, repo.StartFrom != null))
                .Select(changeSets => changeSets.Count())
                .Sum();

            var skippedFirstCommits = _repositories.Count() - 1;
            var skippedLastCommits = _repositories.Count(repo => repo.RenamedFrom);

            return sum - (skippedFirstCommits + skippedLastCommits);
        }

        public void NextChangeSet(Changeset changeSet)
        {
            _current++;

            var currentCommit = new CurrentCommit(changeSet.ChangesetId, changeSet.Comment);

            _mediator.Publish(new ProgressNotification(
                _current,
                _total,
                currentCommit));
        }
    }
}
