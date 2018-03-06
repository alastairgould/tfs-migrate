using System;

namespace TfsMigrate.Core.UseCases.ImportWorkItemAssociations
{
    public interface ICreateWorkItemLink
    {
        void CreateLink(Uri projectCollection, string teamProjectName, string gitRepoName, int workItemId,
            string sha);
    }
}
