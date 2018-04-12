using System;

namespace TfsMigrate.Contracts
{
    public class TfsRepository
    {
        public Uri ProjectCollection { get; set; }

        public string Path { get; set; }
        
        public string StartFrom { get; set; }

        public bool ImportWorkItems { get; set; }

        public bool RenamedFrom { get; set; }

        public bool RenamedTo { get; set; }
    }
}
