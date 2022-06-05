namespace Scrummy.DataAccess.GitLab.Caching
{
    internal class CacheKeyBuilder : ICacheKeyBuilder
    {
        public string ProjectId { get; set; }
        
        public string Key { get; set; }

        public string Build()
        {
            return $"{ProjectId}:{Key}";
        }
    }
}