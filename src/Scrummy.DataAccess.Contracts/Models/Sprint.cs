using System;
using System.Collections.Generic;

namespace Scrummy.DataAccess.Contracts.Models
{
    public class Sprint
    {
        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<Item> Items { get; set; }
    }
}