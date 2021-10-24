using IO.Juenger.GitLab.Client;
using Scrummy.GitLab.Contracts.Configs;

namespace Scrummy.GitLab.Configs
{
    public class GitLabConfig : IGitLabConfig
    {
        public Configuration RestApiConfig { get; set; }

        public string BacklogLabel { get; set; } = "Backlog";
    }
}