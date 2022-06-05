using System;
using Microsoft.Extensions.Configuration;

namespace Scrummy.DataAccess.GitLab.Configs
{
    internal class BacklogProviderConfig : IBacklogProviderConfig
    {
        public string BacklogLabel { get; }

        public BacklogProviderConfig(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            
            var section = configuration.GetSection("GitLab");
            BacklogLabel = section.GetValue("BacklogLabel", "Backlog");
        }
    }
}