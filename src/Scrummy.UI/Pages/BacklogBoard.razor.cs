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
//         [Parameter] 
//         public IEnumerable<Item> Items { get; set; }
//         
//         private IEnumerable<Item> _backlogItems;
//
//         private string _dropClass = "";
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
//     }
// }