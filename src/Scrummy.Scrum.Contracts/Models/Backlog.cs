using System.Collections.Generic;

namespace Scrummy.Scrum.Contracts.Models
{
    public class Backlog
    {
        public Dictionary<string, List<Item>> Stages { get; set; }
    }
}