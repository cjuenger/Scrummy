using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class ItemComponent
    {
        [Parameter] public Item Item { get; set; }
        
        [CascadingParameter] protected BoardComponent Board { get; set; }
        
        private void FollowLink()
        {
            var link = Item.Link;
            // TODO: 20210815 CJ: Open link in new tab!
        }
        
        private void HandleDragStarted(DragEventArgs e)
        {
            Board.DragItem = Item;
            Debug.WriteLine($"Started dragging item {Item.Title}");
        }
        
        private void HandleDragEnter()
        {
            if(Item == Board.DragItem) return;
            
            Board.Items.Remove(Board.DragItem);
            var index = Board.Items.IndexOf(Item);
            Board.Items.Insert(index, Board.DragItem);
            

            // if (ListStatus == Container.Payload.Status) return;
            //
            // if (AllowedStatuses != null && !AllowedStatuses.Contains(Container.Payload.Status))
            // {
            //     _dropClass = "no-drop";
            // }
            // else
            // {
            //     _dropClass = "can-drop";
            // }
        }
    }
}