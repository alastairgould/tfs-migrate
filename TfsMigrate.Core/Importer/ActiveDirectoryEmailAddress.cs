namespace TfsMigrate.Core.Importer
{
    public class ActiveDirectoryEmailAddress : IFindEmailAddress
    {
        public string EmailAddressForUser(string userName)
        {
            return "no.user@example.com";
        }
    }
}
