using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.UI.Shared;

namespace Scrummy.UI.Services
{
    public class DragAndDropService : IDragAndDropService
    {
        private const string DraggedItemStyle = "margin-left: 5px; opacity: 0.5;";

        private Timer _debounceTimer;
        
        // private ItemComponent _enteredItemComponent;

        private ItemComponent _draggedItemComponent;
        private ItemComponent _lastEnteredItemComponent;
        private List<Item> _currentList;

        private Item _dragItem;
        private List<Item> _previousList;

        private int _enteredVsLeaves;

        public List<Item> CurrentList
        {
            get => _currentList;

            set
            {
                if (_currentList != null)
                {
                    _previousList = _currentList;
                }

                _currentList = value;
            }
        }

        public void HandleItemDragStarted(ItemComponent itemComponent)
        {
            Debug.WriteLine($"Started {itemComponent.Id}");

            return;

            _dragItem = itemComponent.Item;
            _draggedItemComponent = itemComponent;
            _lastEnteredItemComponent = itemComponent;
            _lastEnteredItemComponent.Style = DraggedItemStyle;
        }

        public void HandleItemDragEnded(ItemComponent itemComponent)
        {
            Debug.WriteLine($"Ended {itemComponent.Id}");

            return;
            
            Task.Run(
                async () =>
                {
                    await Task.Delay(1000);
                    itemComponent.Style = "";
                    _lastEnteredItemComponent.Style = "";
                });

            itemComponent.Style = "margin: 100px;";
            _lastEnteredItemComponent.Style = "margin: 100px;";
            
            // _lastEnteredItemComponent.Style = "";
            // _draggedItemComponent.Style = "";
        }

        public void HandleItemDragEnter(ItemComponent itemComponent)
        {
            if (itemComponent.Item.Equals(_dragItem))
            {
                Debug.WriteLine($"Entering {itemComponent.Id}: {_enteredVsLeaves}");
                return;
            }
            
            _enteredVsLeaves++;
            Debug.WriteLine($"Entering {itemComponent.Id}: {_enteredVsLeaves}");

            return;
            
            var index = CurrentList.IndexOf(itemComponent.Item);

            if (_previousList != null)
            {
                _previousList.Remove(_dragItem);
                _previousList = null;
            }
            else
            {
                CurrentList.Remove(_dragItem);
            }

            if (index < CurrentList.Count)
            {
                CurrentList.Insert(index, _dragItem);
            }
            else
            {
                CurrentList.Add(_dragItem);
            }
            
            _lastEnteredItemComponent.Style = "";
            _lastEnteredItemComponent = itemComponent;
            _lastEnteredItemComponent.Style = DraggedItemStyle;
            
            Debug.WriteLine($"Drag enter: item {itemComponent.Id} style={itemComponent.Style}");
        }

        public void HandleItemDragLeave(ItemComponent itemComponent)
        {
            if (itemComponent.Item.Equals(_dragItem))
            {
                Debug.WriteLine($"Leaving {itemComponent.Id}: {_enteredVsLeaves}");
                return;
            }
            
            _enteredVsLeaves--;
            Debug.WriteLine($"Leaving {itemComponent.Id}: {_enteredVsLeaves}");

            return;
            
            if (_enteredVsLeaves <= 0)
            {
                _lastEnteredItemComponent.Style = "";
            }
        }

        public void HandleItemDrop(ItemComponent itemComponent)
        {
            Debug.WriteLine($"Dropped at component {itemComponent.Id}");
            itemComponent.Style = "";
        }
    }
}