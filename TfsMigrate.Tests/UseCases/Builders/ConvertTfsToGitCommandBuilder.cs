
using System;
using System.Collections.Generic;
using TfsMigrate.Contracts;
using TfsMigrate.Core.UseCases.ConvertTfsToGit;

namespace TfsMigrate.Tests.UseCases.Builders
{
    public class ConvertTfsToGitCommandBuilder
    {
        public IEnumerable<TfsRepository> TfsRepositories { get; set; }

        public string OutputDirectory { get; set; }

        public ConvertTfsToGitCommandBuilder()
        {
            TfsRepositories = new List<TfsRepository>()
            {
                new TfsRepository()
                {
                    ImportWorkItems = false,
                    Path = @"$Test/Test",
                    ProjectCollection = new Uri("http://www.test.com"),
                    RenamedFrom = false,
                    RenamedTo = true
                }
            };

            OutputDirectory = @"C:\TestDir";
        }

        public ConvertTfsToGitCommand CreateCommand()
        {
            return new ConvertTfsToGitCommand(TfsRepositories, OutputDirectory);
        }
    }
}
