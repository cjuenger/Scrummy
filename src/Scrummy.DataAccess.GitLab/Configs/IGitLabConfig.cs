namespace Scrummy.DataAccess.GitLab.Configs
{
    public interface IGitLabConfig
    {
        string ProjectId { get; set; }

        public string AccessToken { get; set; }
    }
}