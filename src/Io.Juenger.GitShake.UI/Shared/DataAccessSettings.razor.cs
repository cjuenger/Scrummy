using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.UI.Configs;

namespace Scrummy.UI.Shared
{
    public partial class DataAccessSettings
    {
        [Inject]
        private IDataAccessConfig DataAccessConfig { get; set; }
        
        private string ProjectId
        {
            get => DataAccessConfig.ProjectId; 
            set => DataAccessConfig.ProjectId = value;
        }

        private string AccessToken
        {
            get => DataAccessConfig.AccessToken;
            set => DataAccessConfig.AccessToken = value;
        }

        private string BaseUrl
        {
            get => DataAccessConfig.BaseUrl; 
            set => DataAccessConfig.BaseUrl = value;
        }
    }
}