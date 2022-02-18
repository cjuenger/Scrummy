using System.Collections.Generic;

namespace Scrummy.Scrum.Contracts.Models
{
    public class DataSeries<TX, TY>
    {
        public string Title { get; set; }

        public IList<Xy<TX, TY>> Series { get; set; }
    }
}