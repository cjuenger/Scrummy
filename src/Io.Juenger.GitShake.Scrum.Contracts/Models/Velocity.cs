using System;

namespace Scrummy.Scrum.Contracts.Models
{
    public class Velocity
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        
        public float AverageVelocity { get; set; }
        public float DayAverageVelocity { get; set; }

        public float Best3SprintsAverageVelocity { get; set; }
        public float Best3SprintsDayAverageVelocity { get; set; }
        
        public float Worst3SprintsAverageVelocity { get; set; }
        public float Worst3SprintsDayAverageVelocity { get; set; }

        public int CountOfSprints { get; set; }

        public float AverageSprintLength { get; set; }
    }
}