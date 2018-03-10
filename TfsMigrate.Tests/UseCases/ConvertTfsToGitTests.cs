using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Xunit;
using MediatR;
using NSubstitute;
using TfsMigrate.Contracts;
using TfsMigrate.Core.Exporter;
using TfsMigrate.Core.Importer;
using TfsMigrate.Core.UseCases.ConvertTfsToGit;
using TfsMigrate.Tests.UseCases.Builders;

namespace TfsMigrate.Tests.UseCases
{
    public class ConvertTfsToGitTests
    {
        [Fact]
        public async Task Given_A_ConvertTfsToGitCommand_With_A_Null_Repository_Directory_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_ThrownAsync()
        {
            var commandBuilder = new ConvertTfsToGitCommandBuilder { OutputDirectory = null };
            commandBuilder.OutputDirectory = null;
            var command = commandBuilder.CreateCommand();
            var commandHandler = CreateCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_ConvertTfsToGitCommand_With_A_Empty_Repository_Directory_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_ThrownAsync()
        {
            var commandBuilder = new ConvertTfsToGitCommandBuilder { OutputDirectory = ""};
            var command = commandBuilder.CreateCommand();
            var commandHandler = CreateCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_ConvertTfsToGitCommand_With_A_Empty_Repository_Directory_With_Whitespace_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_ThrownAsync()
        {
            var commandBuilder = new ConvertTfsToGitCommandBuilder { OutputDirectory = "" };
            var command = commandBuilder.CreateCommand();
            var commandHandler = CreateCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_ConvertTfsToGitCommand_With_A_Null_TfsRespository_List_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_ThrownAsync()
        {
            var commandBuilder = new ConvertTfsToGitCommandBuilder { TfsRepositories = null};
            var command = commandBuilder.CreateCommand();
            var commandHandler = CreateCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_ConvertTfsToGitCommand_With_A_Empty_TfsRespository_List_When_The_Command_Is_Handled_Then_A_Validation_Exception_Is_ThrownAsync()
        {
            var commandBuilder = new ConvertTfsToGitCommandBuilder { TfsRepositories = new TfsRepository[0] };
            var command = commandBuilder.CreateCommand();
            var commandHandler = CreateCommandHandler();

            var exception = await Record.ExceptionAsync(() => commandHandler.Handle(command, new CancellationToken()));

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Given_A_ConvertTfsToGitCommand_When_The_Command_Is_Handled_Then_A_Git_Repository_Is_Created()
        {
            var gitClientFactory = Substitute.For<Func<string, IGitClient>>();
            var commandBuilder = new ConvertTfsToGitCommandBuilder();
            var command = commandBuilder.CreateCommand();
            var commandHandler = CreateCommandHandler(createGitClient: gitClientFactory);

            await commandHandler.Handle(command, new CancellationToken());

            gitClientFactory.Received()(commandBuilder.OutputDirectory).Init();
        }

        [Fact]
        public async Task Given_A_ConvertTfsToGitCommand_When_The_Command_Is_Handled_Then_A_GitWriterStreamIsCreated()
        {
            var gitClientFactory = Substitute.For<Func<string, IGitClient>>();
            var commandBuilder = new ConvertTfsToGitCommandBuilder();
            var command = commandBuilder.CreateCommand();
            var commandHandler = CreateCommandHandler(createGitClient: gitClientFactory);

            await commandHandler.Handle(command, new CancellationToken());

            gitClientFactory.Received()(commandBuilder.OutputDirectory).CreateGitStreamWriter();
        }

        public ConvertTfsToGitCommandHandler CreateCommandHandler(IMediator mediator = null,
            Func<string, IGitClient> createGitClient = null, 
            IRetriveChangeSets retriveChangeSets = null)
        {
            if (mediator == null)
            {
                mediator = Substitute.For<IMediator>();
            }

            if (createGitClient == null)
            {
                createGitClient = Substitute.For<Func<string, IGitClient>>();
            }

            if (retriveChangeSets == null)
            {
                retriveChangeSets = Substitute.For<IRetriveChangeSets>();
            }

            return new ConvertTfsToGitCommandHandler(mediator, createGitClient, retriveChangeSets);
        }
    }
}
