using IO.Juenger.GitLab.Api;

namespace Scrummy.DataAccess.GitLab.Providers
{
    public interface IProjectApiProvider
    {
        IProjectApi ProjectApi { get; }
    }
}