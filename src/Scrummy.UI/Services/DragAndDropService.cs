using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.UI.Services
{
    public class DragAndDropService<T> : IDragAndDropService<T>
    {
        private double _previousX;
        private double _previousY;
        
        public T DragItem { get; set; }
        
        public List<T> Items { get; set; }
        
        public void HandleDragStarted(T item, double x, double y)
        {
            Debug.WriteLine($"Dragging item {item} started at x={x} y={y}");
            
            _previousX = x;
            _previousY = y;
            
            DragItem = item;
            Items.Remove(item);
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