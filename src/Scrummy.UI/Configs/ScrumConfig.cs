using System;

namespace Scrummy.UI.Configs
{
    public class ScrumConfig : IScrumConfig
    {
        public DateTime ProjectStart { get; set; }
        public int SprintLength { get; set; }
    }
}