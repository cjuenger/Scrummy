using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Scrummy.DataAccess.GitLab.GraphQl.Responses;

namespace Scrummy.DataAccess.GitLab.GraphQl.Queries
{
    internal class BoardQueries : IBoardQueries
    {
        private readonly GraphQLHttpClient _graphQlClient;
        
        public BoardQueries()
        {
            _graphQlClient = new GraphQLHttpClient(
                "https://gitlab.com/api/graphql", 
                new NewtonsoftJsonSerializer());

            const string apiToken = "67QbbetYYda4fSujaLfF";
            _graphQlClient
                .HttpClient
                .DefaultRequestHeaders.Add("Authorization", $"bearer {apiToken}");
        }

        public async Task<Project> GetBoardsAsync(string projectPath)
        {
            var boardRequest = new GraphQLRequest(CreateBoardsQuery(projectPath));
            var response = await _graphQlClient.SendQueryAsync<ProjectResponse>(boardRequest);
            return response.Data.Project;
        }
        
        public async Task<Project> GetBoardWithListsAsync(string projectPath, string boardId)
        {
            var boardRequest = new GraphQLRequest(CreateBoardWithListsQuery(projectPath, boardId));
            var response = await _graphQlClient.SendQueryAsync<ProjectResponse>(boardRequest);
            return response.Data.Project;
        }

        private static string CreateBoardsQuery(string projectPath)
        {
            return @"
            {
                project(fullPath: """ + projectPath + @""") 
                {
                    id
                    name
                    boards {
                        edges {
                            node {
                                id
                                name
                            }
                        }
                    }
                }
            }";
        }
        
        private static string CreateBoardWithListsQuery(string projectPath, string boardId)
        {
            return @"
                {
	                project(fullPath: """ + projectPath + @""")
                    {   
                        id
                        name
                        board(id: ""gid://gitlab/Board/" + boardId + @""") 
                        {
                            id
                            name
                            lists 
                            {
    	                        edges 
                                {
    	                            node 
                                    {
    	                                id
                                        title
                                        issues 
                                        {
                                            edges 
                                            {
                                                node 
                                                {
                                                    id
                                                    title
                                                    description
                                                    webUrl
                                                    state
                                                    createdAt
                                                    closedAt
                                                    labels 
                                                    {
                                                        edges 
                                                        {
                                                            node 
                                                            {
                                                                title
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
    	                            }
    	                        }
    	                    } 
                        }
                    }
                }";
        }
    }
}