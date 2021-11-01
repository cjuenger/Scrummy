using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Exceptions;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.DataAccess.Contracts.Providers;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.DataAccess.GitLab.GraphQl.Queries;
using Scrummy.DataAccess.GitLab.GraphQl.Responses;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class BacklogProvider : IBacklogProvider
    {
        private readonly IBacklogProviderConfig _backlogProviderConfig;
        private readonly IBoardQueries _boardQueries;

        public BacklogProvider(IBacklogProviderConfig backlogProviderConfig, IBoardQueries boardQueries)
        {
            _backlogProviderConfig = backlogProviderConfig ?? throw new ArgumentNullException(nameof(backlogProviderConfig));
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
                .Where(be => be.Board.Name == _backlogProviderConfig.BacklogLabel)
                .Select(be => be.Board)
                .FirstOrDefault();

            if (backlogBoard == null)
            {
                throw new MissingBoardException($"The board {_backlogProviderConfig.BacklogLabel} does not exist!");
            }

            project = await _boardQueries
                .GetBoardWithListsAsync(projectPath, backlogBoard.Id)
                .ConfigureAwait(false);
            
            return project.Board;
        }
    }
}