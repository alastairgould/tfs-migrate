using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using TfsMigrate.Core.CommitTree;
using TfsMigrate.Core.GitFastImport;
using TfsMigrate.Core.Tfs;

namespace TfsMigrate
{
    public class Program
    {
        private static HashSet<int> _SkipCommits = new HashSet<int>()
        {
            // use for skipping checkins that are unnecessary/outside the scope of branching
            // one example is build templates for TFS
        };

        private static HashSet<int> _BreakCommits = new HashSet<int>()
        {
            // use this for debugging when you want to stop at a particular checkin for analysis
        };

        static void Main(string[] args)
        {
            var collection = new TfsTeamProjectCollection(new Uri(""));
            collection.EnsureAuthenticated();
            var versionControl = collection.GetService<VersionControlServer>();

            var allChanges = versionControl
                .QueryHistory(
                    "",
                    VersionSpec.Latest,
                    0,
                    RecursionType.Full,
                    null,
                    new ChangesetVersionSpec(1),
                    VersionSpec.Latest,
                    int.MaxValue,
                    true,
                    false)
                .OfType<Changeset>()
                .OrderBy(x => x.ChangesetId)
                .ToList();


            var branches = new Dictionary<string, Tuple<string, CommitNode>>();


            var outStream = ProcessRunner();

            foreach (var changeSet in allChanges)
            {
                var commitTreeGenerator = new TfsCommitTreeGenerator();
                var commitTree = commitTreeGenerator.CreateCommitTree(changeSet, branches);

                var gitFastImportGenerator = new GitFastImport(outStream.BaseStream);
                gitFastImportGenerator.ProccessCommit(commitTree);
            }

            outStream.Close();
        }

        private static StreamWriter ProcessRunner()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WorkingDirectory = @"";

            Process process = Process.Start(processStartInfo);

            if (process != null)
            {
                process.StandardInput.WriteLine("git init");
                process.StandardInput.WriteLine("git fast-import");
            }

            return process.StandardInput;
        }

        private static StreamWriter FileStream()
        {
            FileStream fs = File.Create(@"");
            return new StreamWriter(fs);
        }
    }
}
