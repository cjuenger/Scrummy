using System;

namespace Scrummy.Scrum.Contracts.Models
{
    public class Throughput
    {
        public TimeSpan AverageStoryPassThroughTime { get; set; }
        public TimeSpan BestStoryPassThroughTime { get; set; }
        public TimeSpan WorstStoryPassThroughTime { get; set; }
        
        public TimeSpan AverageBugPassThroughTime { get; set; }
        public TimeSpan BestBugPassThroughTime { get; set; }
        public TimeSpan WorstBugPassThroughTime { get; set; }
        
        public TimeSpan AverageOtherPassThroughTime { get; set; }
        public TimeSpan BestOtherPassThroughTime { get; set; }
        public TimeSpan WorstOtherPassThroughTime { get; set; }
    }
}