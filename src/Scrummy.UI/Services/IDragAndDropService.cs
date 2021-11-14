using System.Collections.Generic;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.UI.Shared;

namespace Scrummy.UI.Services
{
    public interface IDragAndDropService
    {
        List<Item> CurrentList { get; set; }
        void HandleItemDragStarted(ItemComponent itemComponent);
        void HandleItemDragEnded(ItemComponent itemComponent);
        void HandleItemDragEnter(ItemComponent itemComponent);
        void HandleItemDragLeave(ItemComponent itemComponent);
        void HandleItemDrop(ItemComponent itemComponent);
    }
}