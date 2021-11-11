using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Components.Web;

namespace Scrummy.UI.Services
{
    public class DragAndDropService<T> : IDragAndDropService<T>
    {
        private double _previousX;
        private double _previousY;

        private T _lastEnteredItem;
        
        public T DragItem { get; set; }
        
        public List<T> Items { get; set; }
        
        public void HandleDragStarted(T item, DragEventArgs e)
        {
            Debug.WriteLine($"Event type {e?.Type}");

            _previousX = e?.ScreenX ?? 0d;
            _previousY = e?.ScreenY ?? 0d;
            
            Debug.WriteLine($"Dragging item {item} started at x={_previousX} y={_previousY}");
            
            DragItem = item;
            // Items.Remove(item);
        }
        
        public void HandleDragEnded(T draggedItem, double x, double y)
        {
            Debug.WriteLine($"Dragging item {draggedItem} ended at x={x} y={y}");

            if (_lastEnteredItem is null || _lastEnteredItem.Equals(default))
            {
                Items.Add(draggedItem);
            }

            _lastEnteredItem = default;
        }

        public void HandleDragEnter(T enteredItem, double x, double y)
        {
            Debug.WriteLine($"Dragged item {DragItem} entered {enteredItem} at at x={x} y={y}");
            
            if(enteredItem.Equals(DragItem)) return;
            
            var index = GetDragIndex(enteredItem, x, y);
            Items.Remove(DragItem);
            if (index < Items.Count)
            {
                Items.Insert(index, DragItem);
            }
            else
            {
                Items.Add(DragItem);
            }

            _lastEnteredItem = enteredItem;
        }

        public void HandleDragLeave(T item)
        {
            if(item.Equals(DragItem)) return;
            if (!Items.Last().Equals(item)) return;
            
            Items.Add(DragItem);
        }
        
        private int GetDragIndex(T item, double x, double y)
        {
            var deltaX = x - _previousX;
            var deltaY = y - _previousY;
            
            var isUpDirection = double.IsNegative(deltaY);
            
            var index = Items.IndexOf(item);

            _previousX = x;
            _previousY = y;
            
            index += (isUpDirection ? 0 : 1);
            index = index < 0 ? 0 : index;
            
            return index;
        }
    }
}