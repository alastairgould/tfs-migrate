using System;
using MediatR;

namespace TfsMigrate.Core.UseCases.ImportWorkItemAssociation.Events
{
    public class ProgressNotification : INotification
    {
        public int PercentComplete { get; }

        public string CommitSha { get; }

        public int CurrentAmount { get; }

        public int AmountToProccess { get; }

        public ProgressNotification(int currentAmount, int amountToProccess, string commitSha)
        {
            PercentComplete = (int)Math.Round(((double)currentAmount / (double)amountToProccess) * 100d);
            CommitSha = commitSha;
            CurrentAmount = currentAmount;
            AmountToProccess = amountToProccess;
        }
    }
}
