using System.Collections.Generic;
using System.Linq;
using MediatR;
using Microsoft.TeamFoundation.VersionControl.Client;
using TfsMigrate.Contracts;
using TfsMigrate.Core.UseCases.ConvertTfsToGit.Events;

namespace TfsMigrate.Core.Importer
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
            return _repositories
                .Select(repo => _retriveChangeSets.RetriveChangeSets(repo.ProjectCollection, repo.Path))
                .Select(changeSets => changeSets.Count())
                .Sum();
        }

        public void NextChangeSet(Changeset changeSet)
        {
            var currentCommit = new CurrentCommit(changeSet.ChangesetId, changeSet.Comment);

            _mediator.Publish(new ProgressNotification(
                _current,
                _total,
                currentCommit));

            _current++;
        }
    }
}
