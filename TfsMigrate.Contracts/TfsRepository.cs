using System;

namespace TfsMigrate.Contracts
{
    public class TfsRepository
    {
        public Uri ProjectCollection { get; set; }

        public string Path { get; set; }

        public bool ImportWorkItems { get; set; }
    }
}
