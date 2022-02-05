namespace Scrummy.DataAccess.GitLab.Configs
{
    public class GitLabConfig : IGitLabConfig
    {
        public string ProjectId { get; set; }
        
        public string AccessToken { get; set; }
    }
}