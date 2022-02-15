﻿using System;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Client;
using Microsoft.Extensions.Configuration;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Configs;

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
            var basePath = configurationSection.GetValue("BasePath", "https://gitlab.com/api");
            var apiToken = _dataAccessConfig.AccessToken ?? configurationSection.GetValue<string>("AccessToken", null);
            var config = new Configuration { BasePath = basePath, AccessToken = apiToken };
            return config;
        }
    }
}