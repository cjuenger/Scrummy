using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class ItemComponent
    {
        // private double _previousX;
        // private double _previousY;
        
        [Parameter] public Item Item { get; set; }
        
        [CascadingParameter] protected BoardComponent Board { get; set; }
        
        private void FollowLink()
        {
            // var link = Item.Link;
            // TODO: 20210815 CJ: Open link in new tab!
        }
        
        private void HandleDragStarted(DragEventArgs e)
        {
            // Debug.WriteLine($"Dragging item {Item.Title} started at x={e.ScreenX} y={e.ScreenY}");

            // _previousX = e.ScreenX;
            // _previousY = e.ScreenY;
            
            Board.DragAndDropService.HandleDragStarted(Item, e.ScreenX, e.ScreenY);
            
            // Board.DragItem = Item;
            // Board.Items.Remove(Board.DragItem);
        }

        private void HandleDragEnded(DragEventArgs e)
        {
            // Debug.WriteLine($"Dragging item {Board.DragItem.Title} ended at x={e.ScreenX} y={e.ScreenY}");
        }
        
        private void HandleDragEnter(DragEventArgs e)
        {
            Board.DragAndDropService.HandleDragEnter(Item, e.ScreenX, e.ScreenY);
            
            // Debug.WriteLine($"Dragged item {Board.DragItem.Title} entered {Item.Title} at at x={e.ScreenX} y={e.ScreenY}");
            //
            // if(Item == Board.DragItem) return;
            //
            // var index = GetDragIndex(e);
            // Board.Items.Remove(Board.DragItem);
            // if (index < Board.Items.Count)
            // {
            //     Board.Items.Insert(index, Board.DragItem);
            // }
            // else
            // {
            //     Board.Items.Add(Board.DragItem);
            // }
        }

        private void HandleDragLeave(DragEventArgs e)
        {
            // Debug.WriteLine($"Dragged item {Board.DragItem.Title} leaves {Item.Title} at at x={e.ScreenX} y={e.ScreenY}");

            Board.DragAndDropService.HandleDragLeave(Item);
            
            // if(Item == Board.DragItem) return;
            // if (Board.Items.Last() != Item) return;
            //
            // Board.Items.Add(Board.DragItem);
        }

        // private int GetDragIndex(DragEventArgs e)
        // {
        //     var currentX = e.ScreenX;
        //     var currentY = e.ScreenY;
        //
        //     var deltaX = currentX - _previousX;
        //     var deltaY = currentY - _previousY;
        //     
        //     var isUpDirection = double.IsNegative(deltaY);
        //     
        //     var index = Board.Items.IndexOf(Item);
        //
        //     _previousX = currentX;
        //     _previousY = currentY;
        //     
        //     index += (isUpDirection ? 0 : 1);
        //     index = index < 0 ? 0 : index;
        //
        //     Debug.WriteLine($"Dragged {(isUpDirection ? "up" : "down")} to index={index} from x={_previousX} y={_previousY} to x={currentX} y={currentY} by ∆x={deltaX} ∆y={deltaY}");
        //     
        //     return index;
        // }
    }
}