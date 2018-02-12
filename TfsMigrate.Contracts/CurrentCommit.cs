namespace TfsMigrate.Contracts
{
    public class CurrentCommit
    {
        public int Id { get; }

        public string CommitDescription { get; }

        public CurrentCommit(int id, string commitDescription)
        {
            Id = id;
            CommitDescription = commitDescription;
        }

        public override string ToString()
        {
            return $"[{Id}] - {CommitDescription}";
        }
    }
}
