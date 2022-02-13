namespace Scrummy.Scrum.Contracts.Models
{
    public class Story : Item
    {
        public string UserStory { get; set; }

        public int? StoryPoints { get; set; }
    }
}