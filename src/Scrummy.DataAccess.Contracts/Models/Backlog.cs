using System.Collections.Generic;

namespace Scrummy.DataAccess.Contracts.Models
{
    public class Backlog
    {
        public Dictionary<string, List<Item>> Stages { get; set; }
    }
}