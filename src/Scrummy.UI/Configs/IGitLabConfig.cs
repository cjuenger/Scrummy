namespace Scrummy.UI.Configs
{
    public interface IGitLabConfig
    {
        string ProjectId { get; set; }

        public string AccessToken { get; set; }
    }
}