using System;
using Io.Juenger.GitLabClient.Api;
using Io.Juenger.GitLabClient.Client;
using Microsoft.Extensions.Configuration;
using Scrummy.DataAccess.Contracts.Interfaces;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class ProjectApiProvider : IProjectApiProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IDataAccessConfig _dataAccessConfig;
        public IProjectApi ProjectApi => new ProjectApi(GetGitLabApiConfiguration());

        public ProjectApiProvider(IConfiguration configuration, IDataAccessConfig dataAccessConfig)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dataAccessConfig = dataAccessConfig ?? throw new ArgumentNullException(nameof(dataAccessConfig));
        }
        
        private Configuration GetGitLabApiConfiguration()
        {
            var configurationSection = _configuration.GetSection("GitLab");

            var basePath = _dataAccessConfig.BaseUrl; 
            if (string.IsNullOrWhiteSpace(basePath))
            {
                basePath = configurationSection.GetValue("BasePath", "https://gitlab.com");
            }
            basePath += "/api";

            var apiToken = _dataAccessConfig.AccessToken;
            if (string.IsNullOrWhiteSpace(apiToken))
            {
                apiToken = configurationSection.GetValue<string>("AccessToken", null);
            }
            
            var config = new Configuration { BasePath = basePath, AccessToken = apiToken };
            
            return config;
        }
    }
}