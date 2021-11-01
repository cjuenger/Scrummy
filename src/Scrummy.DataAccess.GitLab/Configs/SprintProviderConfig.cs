using Microsoft.Extensions.Configuration;

namespace Scrummy.DataAccess.GitLab.Configs
{
    internal class SprintProviderConfig : ISprintProviderConfig
    {
        private const string DefaultTimePattern = @"(From:|To:)\s*(\d{4})-(\d{2})-(\d{2})(T(\d{2}):(\d{2})(:(\d{2}))?)?";
        private const string DefaultSprintLabelPattern = "(Sprint|sprint)";
        public string SprintTimePattern { get; }
        public string SprintLabelPattern { get; }

        public SprintProviderConfig(IConfiguration configuration)
        {
            var section = configuration.GetSection("GitLab");

            SprintTimePattern = section.GetValue("SprintTimePattern",DefaultTimePattern);
            SprintLabelPattern = section.GetValue("SprintLabelPattern", DefaultSprintLabelPattern);
        }
    }
}