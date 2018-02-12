﻿using MediatR;
using System;
using TfsMigrate.Contracts;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit.Events
{
    public class ProgressNotification : INotification
    {
        public int PercentComplete { get; }

        public CurrentCommit CurrentCommit { get; }

        public int CurrentAmount { get; }

        public int AmountToProccess { get; }

        public ProgressNotification(int currentAmount, int amountToProccess, CurrentCommit currentCommit)
        {
            this.PercentComplete = (int)Math.Round(((double)currentAmount / (double)amountToProccess) * 100d);
            this.CurrentCommit = currentCommit;
            this.CurrentAmount = currentAmount;
            this.AmountToProccess = amountToProccess;
        }
    }
}
