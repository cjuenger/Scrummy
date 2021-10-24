using Scrummy.GitLab.Contracts.Configs;

namespace Scrummy.GitLab.Configs
{
    public class GitLabConfig : IGitLabConfig
    {
        public string BacklogLabel { get; set; } = "Backlog";
    }
}