using Scrummy.DataAccess.Contracts.Configs;

namespace Scrummy.DataAccess.GitLab.Configs
{
    internal class GitLabConfig : IGitLabConfig
    {
        public string BacklogLabel { get; set; } = "Backlog";
    }
}