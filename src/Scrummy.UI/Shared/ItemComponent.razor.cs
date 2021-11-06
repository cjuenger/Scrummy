using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class ItemComponent
    {
        private Item _dragLeaveItem = null;
        
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
            Board.Items.Remove(Board.DragItem);
            Debug.WriteLine($"Started dragging item {Item.Title}");
        }
        
        private void HandleDragEnter()
        {
            if(Item == Board.DragItem) return;
            
            Board.Items.Remove(Board.DragItem);
            var index = Board.Items.IndexOf(Item);
            
            index += GetDragDirection();
            index = index < 0 ? 0 : index;
            
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

        private void HandleDragLeave()
        {
            _dragLeaveItem = Item;
            
            if(Item == Board.DragItem) return;
            if (Board.Items.Last() != Item) return;
            
            Board.Items.Remove(Board.DragItem);
            Board.Items.Add(Board.DragItem);
        }

        private int GetDragDirection()
        {
            var index = Board.Items.IndexOf(Board.DragItem);
            var dragLeaveIndex = Board.Items.IndexOf(_dragLeaveItem);

            return index >= dragLeaveIndex ? 1 : 0;
        }
    }
}