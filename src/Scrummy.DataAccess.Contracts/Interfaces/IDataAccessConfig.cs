namespace Scrummy.DataAccess.Contracts.Interfaces
{
    public interface IDataAccessConfig
    {
        string ProjectId { get; set; }

        public string AccessToken { get; set; }
    }
}