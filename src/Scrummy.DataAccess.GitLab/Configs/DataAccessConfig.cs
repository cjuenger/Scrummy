using Scrummy.DataAccess.Contracts.Interfaces;

namespace Scrummy.DataAccess.GitLab.Configs
{
    internal class DataAccessConfig : IDataAccessConfig
    {
        public string ProjectId { get; set; }
        
        public string AccessToken { get; set; }
        
        public string BaseUrl { get; set; }
    }
}