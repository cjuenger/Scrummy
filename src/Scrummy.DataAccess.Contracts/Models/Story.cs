namespace Scrummy.DataAccess.Contracts.Models
{
    public class Story : Item
    {
        public string UserStory { get; set; }

        public int? StoryPoints { get; set; }
    }
}