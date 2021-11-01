namespace Scrummy.DataAccess.GitLab.Configs
{
    internal interface ISprintProviderConfig
    {
        string SprintTimePattern { get; }
        string SprintLabelPattern { get; }
    }
}