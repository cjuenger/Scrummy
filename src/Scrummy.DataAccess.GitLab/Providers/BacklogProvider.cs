using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Configs;
using Scrummy.DataAccess.Contracts.Exceptions;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.DataAccess.Contracts.Providers;
using Scrummy.DataAccess.GitLab.GraphQl.Queries;
using Scrummy.DataAccess.GitLab.GraphQl.Responses;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class BacklogProvider : IBacklogProvider
    {
        private readonly IGitLabConfig _gitLabConfig;
        private readonly IBoardQueries _boardQueries;

        public BacklogProvider(IGitLabConfig gitLabConfig, IBoardQueries boardQueries)
        {
            _gitLabConfig = gitLabConfig ?? throw new ArgumentNullException(nameof(gitLabConfig));
            _boardQueries = boardQueries ?? throw new ArgumentNullException(nameof(boardQueries));
        }
        
        public async Task<IEnumerable<Item>> GetItemsAsync(string projectPath, CancellationToken ct)
        {
            var backlogBoard = await QueryBacklogBoardAsync(projectPath, ct).ConfigureAwait(false);
            throw new NotImplementedException();
        }

        private async Task<Board> QueryBacklogBoardAsync(string projectPath, CancellationToken ct)
        {
            var project = await _boardQueries
                .GetBoardsAsync(projectPath)
                .ConfigureAwait(false);

            var backlogBoard = project.Boards.BoardsEdges
                .Where(be => be.Board.Name == _gitLabConfig.BacklogLabel)
                .Select(be => be.Board)
                .FirstOrDefault();

            if (backlogBoard == null)
            {
                throw new MissingBoardException($"The board {_gitLabConfig.BacklogLabel} does not exist!");
            }

            project = await _boardQueries
                .GetBoardWithListsAsync(projectPath, backlogBoard.Id)
                .ConfigureAwait(false);
            
            return project.Board;
        }
    }
}