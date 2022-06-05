using System;

namespace Scrummy.UI.Configs
{
    public interface IScrumConfig
    {
        public DateTime ProjectStart { get; set; }

        public int SprintLength { get; set; }
    }
}