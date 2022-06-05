using IO.Juenger.GitLab.Api;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal interface IProjectApiProvider
    {
        IProjectApi ProjectApi { get; }
    }
}