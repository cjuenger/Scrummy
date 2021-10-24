using IO.Juenger.GitLab.Client;

namespace Scrummy.GitLab.Contracts.Configs
{
    public interface IGitLabConfig
    {
        Configuration RestApiConfig { get; set; }

        string BacklogLabel { get; set; }
    }
}