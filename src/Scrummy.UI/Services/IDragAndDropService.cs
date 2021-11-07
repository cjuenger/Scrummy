using System.Collections.Generic;

namespace Scrummy.UI.Services
{
    public interface IDragAndDropService<T>
    {
        T DragItem { get; set; }
        List<T> Items { get; set; }
        void HandleDragStarted(T item, double x, double y);
        void HandleDragEnter(T enteredItem, double x, double y);
        void HandleDragLeave(T item);
    }
}