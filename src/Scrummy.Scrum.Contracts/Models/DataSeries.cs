using System;
using System.Collections.Generic;

namespace Scrummy.Scrum.Contracts.Models
{
    public class DataSeries
    {
        public string Title { get; set; }

        public IList<Xy<DateTime, int>> Series { get; set; }
    }
}