using System;
using System.Collections.Generic;
using TfsMigrate.Contracts;
using TfsMigrate.Powershell;

namespace Tfs
{
    class Program
    {
        static void Main(string[] args)
        {
            var tfsRepos = new List<TfsRepository>()
            {
                new TfsRepository
                {
                    ProjectCollection = new Uri(@"http://tfs.kingsway.asos.com:8080/tfs/ASOS/"),
                    Path = @"$/ASOS/Finance/Vouchers/Int"
                },
                new TfsRepository
                {
                    ProjectCollection = new Uri(@"https://asos.visualstudio.com/DefaultCollection"),
                    Path = @"$/ASOS/Finance/Vouchers/Int"
                }
            }.ToArray();

            var cmdlet = new ConvertToGit
            {
                Repositories = tfsRepos,
                LocalRepositoryPath = @"C:\Users\alastair.gould\testrepo21"
            };

            var al  = cmdlet.Invoke().GetEnumerator().MoveNext();

            System.Console.WriteLine();
        }
    }
}
