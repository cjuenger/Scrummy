using System;

namespace Scrummy.Scrum.Contracts.Models
{
    public class Throughput
    {
        public TimeSpan AverageStoryThroughputTime { get; set; }
        public TimeSpan BestStoryThroughputTime { get; set; }
        public TimeSpan WorstStoryThroughputTime { get; set; }
        
        public TimeSpan AverageBugThroughputTime { get; set; }
        public TimeSpan BestBugThroughputTime { get; set; }
        public TimeSpan WorstBugThroughputTime { get; set; }
        
        public TimeSpan AverageOtherThroughputTime { get; set; }
        public TimeSpan BestOtherThroughputTime { get; set; }
        public TimeSpan WorstOtherThroughputTime { get; set; }
    }
}