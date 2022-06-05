using System;
using Io.Juenger.Common.Util;

namespace Scrummy.Scrum.Contracts.Models
{
    public class SprintInfo
    {
        /// <summary>
        ///     Id of the sprint
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        ///     Name of the sprint
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        ///     Start time of the sprint
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        ///     End time of the sprint
        /// </summary>
        public DateTime EndTime { get; set; }
        
        /// <summary>
        ///     Length of the sprint
        /// </summary>
        public int Length => GetSprintLength();
        
        private int GetSprintLength()
        {
            var businessDays = StartTime.GetBusinessDaysUntil(EndTime);
            return businessDays;
        }
    }
}