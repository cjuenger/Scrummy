// [Inject]
// private IBoardQueries BoardQueries { get; set; }



// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Components.Web;
// using Scrummy.DataAccess.Contracts.Enums;
// using Scrummy.DataAccess.Contracts.Models;
//
// namespace Scrummy.UI.Pages
// {
//     // TODO: 20211101 CJ: Must be refactored!
//     public partial class BacklogBoard
//     {
//         // [Inject]
//         // private IBoardQueries BoardQueries { get; set; }
//         
//         [Parameter] 
//         public IEnumerable<Item> Items { get; set; }
//         
//         private IEnumerable<Item> _backlogItems;
//
//         private string _dropClass = "";
//     
//         protected override async Task OnInitializedAsync()
//         {
//     
//             // var project = await BoardQueries
//             //     .GetBoardsAsync("mygrouptoexploregitlabapi/api_evaluation");
//             //
//             // Debug.WriteLine("Boards:");
//             // foreach (var boardsEdge in project.Boards.BoardsEdges)
//             // {
//             //     Debug.WriteLine($"Board id:{boardsEdge.Board.Id}, name: {boardsEdge.Board.Name}");
//             // }
//             //
//             // project = await BoardQueries.GetBoardWithListsAsync(
//             //     "mygrouptoexploregitlabapi/api_evaluation", 
//             //     "3002641"
//             //     );
//     
//             // Debug.WriteLine($"Board id:{project.Board.Id}, name: {project.Board.Name}");
//             //
//             // foreach (var listsEdge in project.Board.Lists.ListsEdges)                             
//             // {
//             //     Debug.WriteLine($"List id:{listsEdge.List.Id}, title: {listsEdge.List.Title}");
//             //
//             //     foreach (var issuesEdge in listsEdge.List.Issues.IssuesEdges)
//             //     {
//             //         Debug.WriteLine(
//             //             $"Issue id:{issuesEdge.Issue.Id}, " +
//             //             $"title: {issuesEdge.Issue.Title}, " +
//             //             $"state: {issuesEdge.Issue.State}, " +
//             //             $"link: {issuesEdge.Issue.WebUrl}, " +
//             //             $"description: {issuesEdge.Issue.Description}");
//             //     }
//             // }
//
//
//             _backlogItems = new[]
//             {
//                 new Item{Title = "Mock Item 1", State = WorkflowState.Ready},
//                 new Item{Title = "Mock Item 2", State = WorkflowState.Planned},
//                 new Item{Title = "Mock Item 3", State = WorkflowState.Processing},
//                 new Item{Title = "Mock Item 4", State = WorkflowState.Opened},
//                 new Story{Title = "Mock Story 1", State = WorkflowState.Opened},
//                 new Story{Title = "Mock Story 2", State = WorkflowState.Opened},
//                 new Story{Title = "Mock Story 3", State = WorkflowState.Opened},
//                 new Story{Title = "Mock Story 4", State = WorkflowState.Opened},
//             };
//             
//             await base.OnInitializedAsync();
//         }
//     
//         // protected override void OnParametersSet()
//         // {
//         //     base.OnParametersSet();         
//         //     _backlogItems = Items?.Where(IsBacklogItem) ?? Enumerable.Empty<Item>();
//         // }
//     
//         private static bool IsBacklogItem(Item item) => item.State is WorkflowState.Opened or WorkflowState.Ready;
//
//         
//
//         private void HandleDrop(object e)
//         {
//             Debug.WriteLine($"Dropped {e.GetType()}");
//         }
//         
//         private void HandleDragEnter()
//         {
//             _dropClass = "can-drop";
//             
//             // if (ListStatus == Container.Payload.Status) return;
//             //
//             // if (AllowedStatuses != null && !AllowedStatuses.Contains(Container.Payload.Status))
//             // {
//             //     _dropClass = "no-drop";
//             // }
//             // else
//             // {
//             //     _dropClass = "can-drop";
//             // }
//         }
//
//         private void HandleDragLeave()
//         {
//             _dropClass = "";
//         }
//     }
// }