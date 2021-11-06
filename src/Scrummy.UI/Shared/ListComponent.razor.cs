using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class ListComponent
    {
        [Parameter] public string Title { get; set; }
        
        [CascadingParameter] protected BoardComponent Board { get; set; }

        private IEnumerable<Item> Items => Board.Items;

        private string _dropClass = "";
    
        protected override async Task OnInitializedAsync()
        {
    
            // var project = await BoardQueries
            //     .GetBoardsAsync("mygrouptoexploregitlabapi/api_evaluation");
            //
            // Debug.WriteLine("Boards:");
            // foreach (var boardsEdge in project.Boards.BoardsEdges)
            // {
            //     Debug.WriteLine($"Board id:{boardsEdge.Board.Id}, name: {boardsEdge.Board.Name}");
            // }
            //
            // project = await BoardQueries.GetBoardWithListsAsync(
            //     "mygrouptoexploregitlabapi/api_evaluation", 
            //     "3002641"
            //     );
    
            // Debug.WriteLine($"Board id:{project.Board.Id}, name: {project.Board.Name}");
            //
            // foreach (var listsEdge in project.Board.Lists.ListsEdges)                             
            // {
            //     Debug.WriteLine($"List id:{listsEdge.List.Id}, title: {listsEdge.List.Title}");
            //
            //     foreach (var issuesEdge in listsEdge.List.Issues.IssuesEdges)
            //     {
            //         Debug.WriteLine(
            //             $"Issue id:{issuesEdge.Issue.Id}, " +
            //             $"title: {issuesEdge.Issue.Title}, " +
            //             $"state: {issuesEdge.Issue.State}, " +
            //             $"link: {issuesEdge.Issue.WebUrl}, " +
            //             $"description: {issuesEdge.Issue.Description}");
            //     }
            // }

            await base.OnInitializedAsync();
        }
    
        // protected override void OnParametersSet()
        // {
        //     base.OnParametersSet();         
        //     _backlogItems = Items?.Where(IsBacklogItem) ?? Enumerable.Empty<Item>();
        // }
    
        private static bool IsBacklogItem(Item item) => item.State is WorkflowState.Opened or WorkflowState.Ready;
        
        private void HandleDrop(object e)
        {
            Debug.WriteLine($"Dropped {e.GetType()}");
        }
        
        private void HandleDragEnter()
        {
            _dropClass = "can-drop";
        }

        private void HandleDragLeave()
        {
            _dropClass = "";
        }
    }
}