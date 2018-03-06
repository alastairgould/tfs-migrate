using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using NSubstitute;
using TfsMigrate.Contracts;
using Xunit;
using TfsMigrate.Core.UseCases.ImportWorkItemAssociations;
using TfsMigrate.Core.UseCases.ImportWorkItemAssociations.Events;

namespace TfsMigrate.Tests.UseCases
{
    public class ImportWorkItemAssociationsTests
    {
        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_A_Null_VstsRepository_When_The_Request_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandHandler = CreateSut();
            var command = new ImportWorkItemAssociationsCommand(null);

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_A_Null_GitRepository_When_The_Request_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandHandler = CreateSut();
            var command = CreateImportWorkItemAssocationCommand();
            command.VstsGitRepository.GitRepository = null;

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_A_Null_ProjectCollection_When_The_Request_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandHandler = CreateSut();
            var command = CreateImportWorkItemAssocationCommand();
            command.VstsGitRepository.ProjectCollection = null;

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_A_Null_TeamProject_When_The_Request_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandHandler = CreateSut();
            var command = CreateImportWorkItemAssocationCommand();
            command.VstsGitRepository.TeamProject = null;

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_A_Empty_Whitespace_TeamProject_When_The_Request_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandHandler = CreateSut();
            var command = CreateImportWorkItemAssocationCommand();
            command.VstsGitRepository.TeamProject = "  ";

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_A_Empty_TeamProject_When_The_Request_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandHandler = CreateSut();
            var command = CreateImportWorkItemAssocationCommand();
            command.VstsGitRepository.TeamProject = "";

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_A_Null_RepositoryName_When_The_Request_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandHandler = CreateSut();
            var command = CreateImportWorkItemAssocationCommand();
            command.VstsGitRepository.RepositoryName = null;

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_A_Empty_RepositoryName_When_The_Request_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandHandler = CreateSut();
            var command = CreateImportWorkItemAssocationCommand();
            command.VstsGitRepository.RepositoryName = "";

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_A_WorkItemAssociation_When_The_Request_Is_Handled_Then_A_WorkItemAssocationLink_Is_Created()
        {
            var createWorkItemLink = Substitute.For<ICreateWorkItemLink>();
            var commandHandler = CreateSut(createWorkItemLink: createWorkItemLink);
            var command = CreateImportWorkItemAssocationCommand();

            command.VstsGitRepository.GitRepository.CommitWorkItemAssociations = new Dictionary<string, IEnumerable<int>>
            {
                {"356a192b7913b04c54574d18c28d46e6395428ab", new List<int> { 1 } }
            };

            await commandHandler.Handle(command, new CancellationToken());

            createWorkItemLink.Received(1).CreateLink(new Uri("https://www.test.com"), "TeamProject", "RepositoryName", 1, "356a192b7913b04c54574d18c28d46e6395428ab");
        }

        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_Multiple_WorkItemIds_Against_A_Single_Commit_When_The_Request_Is_Handled_Then_A_WorkItemAssocationLink_Is_Created_For_Each_WorkId()
        {
            var createWorkItemLink = Substitute.For<ICreateWorkItemLink>();
            var commandHandler = CreateSut(createWorkItemLink: createWorkItemLink);
            var command = CreateImportWorkItemAssocationCommand();

            command.VstsGitRepository.GitRepository.CommitWorkItemAssociations = new Dictionary<string, IEnumerable<int>>
            {
                {"356a192b7913b04c54574d18c28d46e6395428ab", new List<int> { 1, 2, 3 } }
            };

            await commandHandler.Handle(command, new CancellationToken());

            createWorkItemLink.Received(1).CreateLink(new Uri("https://www.test.com"), "TeamProject", "RepositoryName", 1, "356a192b7913b04c54574d18c28d46e6395428ab");
            createWorkItemLink.Received(1).CreateLink(new Uri("https://www.test.com"), "TeamProject", "RepositoryName", 2, "356a192b7913b04c54574d18c28d46e6395428ab");
            createWorkItemLink.Received(1).CreateLink(new Uri("https://www.test.com"), "TeamProject", "RepositoryName", 3, "356a192b7913b04c54574d18c28d46e6395428ab");
        }

        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_Multiple_Commits_When_The_Request_Is_Handled_Then_A_WorkItemAssocationLink_Is_Created_For_Each_Commit()
        {
            var createWorkItemLink = Substitute.For<ICreateWorkItemLink>();
            var commandHandler = CreateSut(createWorkItemLink: createWorkItemLink);
            var command = CreateImportWorkItemAssocationCommand();

            command.VstsGitRepository.GitRepository.CommitWorkItemAssociations = new Dictionary<string, IEnumerable<int>>
            {
                {"356a192b7913b04c54574d18c28d46e6395428ab", new List<int> { 1 } },
                {"da4b9237bacccdf19c0760cab7aec4a8359010b0", new List<int> { 1 } },
                {"77de68daecd823babbb58edb1c8e14d7106e83bb", new List<int> { 1 } }
            };

            await commandHandler.Handle(command, new CancellationToken());

            createWorkItemLink.Received(1).CreateLink(new Uri("https://www.test.com"), "TeamProject", "RepositoryName", 1, "356a192b7913b04c54574d18c28d46e6395428ab");
            createWorkItemLink.Received(1).CreateLink(new Uri("https://www.test.com"), "TeamProject", "RepositoryName", 1, "da4b9237bacccdf19c0760cab7aec4a8359010b0");
            createWorkItemLink.Received(1).CreateLink(new Uri("https://www.test.com"), "TeamProject", "RepositoryName", 1, "77de68daecd823babbb58edb1c8e14d7106e83bb");
        }

        [Fact]
        public async Task Given_ImportWorkItemAssociationCommand_With_Multiple_Commits_When_The_Request_Is_Handled_Then_A_ProgressNotification_Is_Published_For_Each_Commit()
        {
            var mediator = Substitute.For<IMediator>();
            var commandHandler = CreateSut(mediator: mediator);
            var command = CreateImportWorkItemAssocationCommand();

            command.VstsGitRepository.GitRepository.CommitWorkItemAssociations = new Dictionary<string, IEnumerable<int>>
            {
                {"356a192b7913b04c54574d18c28d46e6395428ab", new List<int> { 1 } },
                {"da4b9237bacccdf19c0760cab7aec4a8359010b0", new List<int> { 1 } },
                {"77de68daecd823babbb58edb1c8e14d7106e83bb", new List<int> { 1 } }
            };

            await commandHandler.Handle(command, new CancellationToken());

            await mediator.Received(1).Publish(Arg.Is<ProgressNotification>(progressNotification =>
                 progressNotification.AmountToProccess == 3 && progressNotification.CurrentAmount == 0 &&
                 progressNotification.CommitSha == "356a192b7913b04c54574d18c28d46e6395428ab" && progressNotification.PercentComplete == 0));

            await mediator.Received(1).Publish(Arg.Is<ProgressNotification>(progressNotification =>
                progressNotification.AmountToProccess == 3 && progressNotification.CurrentAmount == 1 &&
                progressNotification.CommitSha == "da4b9237bacccdf19c0760cab7aec4a8359010b0" && progressNotification.PercentComplete == 33));

            await mediator.Received(1).Publish(Arg.Is<ProgressNotification>(progressNotification =>
                progressNotification.AmountToProccess == 3 && progressNotification.CurrentAmount == 2 &&
                progressNotification.CommitSha == "77de68daecd823babbb58edb1c8e14d7106e83bb" && progressNotification.PercentComplete == 67));
        }

        private static ImportWorkItemAssociationsCommand CreateImportWorkItemAssocationCommand()
        {
            var command = new ImportWorkItemAssociationsCommand(new VstsGitRepository()
            {
                GitRepository = new GitRepository()
                {
                    CommitWorkItemAssociations = new Dictionary<string, IEnumerable<int>>()
                },
                ProjectCollection = new Uri("https://www.test.com"),
                TeamProject = "TeamProject",
                RepositoryName = "RepositoryName"
            });
            return command;
        }

        private static ImportWorkItemAssociationsCommandHandler CreateSut(IMediator mediator = null, ICreateWorkItemLink createWorkItemLink = null)
        {
            if (mediator == null)
            {
                mediator = Substitute.For<IMediator>();
            }

            if (createWorkItemLink == null)
            {
                createWorkItemLink = Substitute.For<ICreateWorkItemLink>();
            }

            var commandHandler = new ImportWorkItemAssociationsCommandHandler(mediator, createWorkItemLink);
            return commandHandler;
        }
    }
}
