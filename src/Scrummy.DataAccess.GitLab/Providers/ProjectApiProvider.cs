using System;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Client;
using Microsoft.Extensions.Configuration;
using Scrummy.DataAccess.GitLab.Configs;

namespace Scrummy.DataAccess.GitLab.Providers
{
    public class ProjectApiProvider : IProjectApiProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IGitLabConfig _gitLabConfig;
        public IProjectApi ProjectApi => new ProjectApi(GetGitLabApiConfiguration());

        public ProjectApiProvider(IConfiguration configuration, IGitLabConfig gitLabConfig)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _gitLabConfig = gitLabConfig ?? throw new ArgumentNullException(nameof(gitLabConfig));
        }
        
        private Configuration GetGitLabApiConfiguration()
        {
            var configurationSection = _configuration.GetSection("GitLab");
            var basePath = configurationSection.GetValue("BasePath", "https://gitlab.com/api");
            var apiToken = _gitLabConfig.AccessToken ?? configurationSection.GetValue<string>("AccessToken", null);
            var config = new Configuration { BasePath = basePath, AccessToken = apiToken };
            return config;
        }
    }
}