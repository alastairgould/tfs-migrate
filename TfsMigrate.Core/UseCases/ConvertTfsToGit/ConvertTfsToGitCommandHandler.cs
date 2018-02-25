using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.TeamFoundation.VersionControl.Client;
using TfsMigrate.Contracts;
using TfsMigrate.Core.CommitTree.Branches;
using TfsMigrate.Core.Exporter;
using TfsMigrate.Core.Importer;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class ConvertTfsToGitCommandHandler : IRequestHandler<ConvertTfsToGitCommand, GitRepository>
    {
        private readonly IRetriveChangeSets _retriveChangeSets;
        private readonly IMediator _mediator;
        private ChangeSetProgressNotifier _progressNotifier;

        public ConvertTfsToGitCommandHandler(IRetriveChangeSets retriveChangeSets,
            IMediator mediator)
        {
            _retriveChangeSets = retriveChangeSets;
            _mediator = mediator;
        }

        public Task<GitRepository> Handle(ConvertTfsToGitCommand convertTfsToGitCommand, CancellationToken cancellationToken)
        {
            var branches = new Branches();

            _progressNotifier = new ChangeSetProgressNotifier(convertTfsToGitCommand.Repositories,
                _retriveChangeSets, _mediator);

            using (var writer = GitStreamWriter.CreateGitStreamWriter(convertTfsToGitCommand.RepositoryDirectory))
            {
                var shouldSkipFirstCommit = false;

                foreach (var reposistory in convertTfsToGitCommand.Repositories)
                {
                    ConvertRepository(branches, writer, reposistory, shouldSkipFirstCommit);
                    shouldSkipFirstCommit = true;
                }
            }

            return Task.FromResult(new GitRepository()
            {
                Path = convertTfsToGitCommand.RepositoryDirectory
            });
        }

        private void ConvertRepository(Branches branches,
            GitStreamWriter writer,
            TfsRepository reposistory,
            bool shouldSkipFirstCommit)
        {
            var changeSets = _retriveChangeSets.RetriveChangeSets(reposistory.ProjectCollection, reposistory.Path);

            if (shouldSkipFirstCommit)
            {
                changeSets = changeSets.Skip(1);
            }

            foreach (var changeSetAndIndex in changeSets.Select((value, i) => new { CurrentIndex = i, ChangeSet = value }))
            {
                _progressNotifier.NextChangeSet(changeSetAndIndex.ChangeSet);
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
    }
}
