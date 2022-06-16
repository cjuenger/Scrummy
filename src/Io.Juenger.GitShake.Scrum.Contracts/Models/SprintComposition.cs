namespace Scrummy.Scrum.Contracts.Models
{
    public class SprintComposition
    {
        public SprintInfo SprintInfo { get; set; }
        public int CountOfStories { get; set; }

        public int CountOfBugs { get; set; }

        public int CountOfOthers { get; set; }
    }
}