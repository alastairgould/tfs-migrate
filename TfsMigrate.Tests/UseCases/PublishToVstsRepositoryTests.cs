using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentValidation;
using NSubstitute;
using TfsMigrate.Core.Exporter;
using TfsMigrate.Core.UseCases.PublishToVstsRepository;
using TfsMigrate.Tests.UseCases.Builders;

namespace TfsMigrate.Tests.UseCases
{
    public class PublishToVstsRepositoryTests
    {
        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_With_A_Null_ProjectCollection_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder {ProjectCollection = null};
            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_With_A_Null_TeamProject_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder { TeamProject = null };
            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_With_A_Empty_TeamProject_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder { TeamProject = "" };
            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_With_A_Empty_TeamProject_With_Whitespace_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder { TeamProject = "  " };
            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_With_A_Null_RepositoryName_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder { RepositoryName = null};
            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_With_A_Empty_RepositoryName_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder { RepositoryName = "" };
            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_With_A_Empty_RepositoryName_With_Whitespace_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder { RepositoryName = "  " };
            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_With_A_Null_Repository_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder { Repository = null};
            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_With_A_Repository_With_A_Null_Path_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder {Repository = {Path = null}};
            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_With_A_Repository_With_A_Empty_Path_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder {Repository = {Path = ""}};
            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_With_A_Repository_With_A_Empty_Path_With_Whitespace_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_Thrown()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder {Repository = {Path = "  "}};
            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_When_The_Command_Is_Handled_Then_A_Vsts_Repository_Is_Created()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder();
            var createVstsGitRepository = Substitute.For<ICreateVstsGitRepository>();

            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler(createVstsGitRepository: createVstsGitRepository);

            await commandHandler.Handle(command, new CancellationToken());

            await createVstsGitRepository.Received().CreateRepository(commandBuilder.ProjectCollection,
                commandBuilder.TeamProject, commandBuilder.RepositoryName, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_When_The_Command_Is_Handled_Then_The_Local_Repository_Is_Pushed_To_The_Remote()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder();
            var gitClientFactory = Substitute.For<Func<string, IGitClient>>();

            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler(createVstsGitRepository: new CreatrVstsGitRepositoryStub(), createGitClient: gitClientFactory);

            await commandHandler.Handle(command, new CancellationToken());

            gitClientFactory(commandBuilder.Repository.Path).Received().AddUpstreamRemote("http://testrepo.com");
            gitClientFactory(commandBuilder.Repository.Path).Received().PushAllUpstreamRemote();
        }

        [Fact]
        public async Task Given_A_PublishToVstsGitRepositoryCommand_When_The_Command_Is_Handled_Then_A_VstsGitRepository_Is_Returned()
        {
            var commandBuilder = new PublishToVstsGitRepositoryCommandBuilder();
            var gitClientFactory = Substitute.For<Func<string, IGitClient>>();

            var command = commandBuilder.CreateCommand();
            var commandHandler = createCommandHandler(createVstsGitRepository: new CreatrVstsGitRepositoryStub(), createGitClient: gitClientFactory);

            var result = await commandHandler.Handle(command, new CancellationToken());

            Assert.Equal(commandBuilder.RepositoryName, result.RepositoryName);
            Assert.Equal(commandBuilder.ProjectCollection, result.ProjectCollection);
            Assert.Equal(commandBuilder.TeamProject, result.TeamProject);
            Assert.Equal(commandBuilder.Repository, result.GitRepository);
        }

        private static PublishToVstsGitRepositoryCommandHandler createCommandHandler(ICreateVstsGitRepository createVstsGitRepository = null, Func<string, IGitClient> createGitClient = null)
        {
            if (createVstsGitRepository == null)
            {
                createVstsGitRepository = Substitute.For<ICreateVstsGitRepository>();
            }

            if (createGitClient == null)
            {
                createGitClient = Substitute.For<Func<string, IGitClient>>();
            }

            return new PublishToVstsGitRepositoryCommandHandler(createVstsGitRepository, createGitClient);
        }

        private class CreatrVstsGitRepositoryStub : ICreateVstsGitRepository
        {
            public Task<string> CreateRepository(Uri projectCollection, string teamProject, string gitRepository, CancellationToken cancellationToken)
            {
                return Task.FromResult("http://testrepo.com");
            }
        }
    }
}
