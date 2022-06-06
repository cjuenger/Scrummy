using Io.Juenger.GitLabClient.Api;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal interface IProjectApiProvider
    {
        IProjectApi ProjectApi { get; }
    }
}