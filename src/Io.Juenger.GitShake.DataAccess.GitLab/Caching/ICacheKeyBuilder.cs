namespace Scrummy.DataAccess.GitLab.Caching
{
    internal interface ICacheKeyBuilder
    {
        string ProjectId { get; set; }
        string Key { get; set; }
        string Build();
    }
}