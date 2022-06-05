using Microsoft.Extensions.Configuration;

namespace Scrummy.DataAccess.GitLab.Configs
{
    internal class SprintProviderConfig : ISprintProviderConfig
    {
        // You can explore the RegEx here https://regexr.com/6f3jm
        private const string DefaultTimePattern = @"(From:|To:)\s*(\d{4})-(\d{2})-(\d{2})(T(\d{2}):(\d{2})(:(\d{2}))?)?";
        
        // You can explore the RegEx here https://regexr.com/6f3j4
        private const string DefaultSprintLabelPattern = "^(Sprint|sprint)((?!(Backlog|backlog)).)*$";
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