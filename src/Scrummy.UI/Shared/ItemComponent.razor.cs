using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class ItemComponent
    {
        private DotNetObjectReference<ItemComponent> _objRef;

        public ItemComponent() => Id = Guid.NewGuid().ToString();
        [Parameter] public Item Item { get; set; }

        public string Id { get; }

        public string Style { get; set; } = "";

        [CascadingParameter] protected BoardComponent Board { get; set; }

        [Inject] private IJSRuntime JsRuntime { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _objRef = DotNetObjectReference.Create(this);
                
                await JsRuntime.InvokeVoidAsync("addDragStartEventHandler", _objRef, Id);
                await JsRuntime.InvokeVoidAsync("addDragEnterEventHandler", _objRef, Id);
                await JsRuntime.InvokeVoidAsync("addDragLeaveEventHandler", _objRef, Id);
                await JsRuntime.InvokeVoidAsync("addDragOverEventHandler", _objRef, Id);
                await JsRuntime.InvokeVoidAsync("addDragEndEventHandler", _objRef, Id);
                await JsRuntime.InvokeVoidAsync("addDropEventHandler", _objRef, Id);
            }
	
            await base.OnAfterRenderAsync(firstRender);
        }

        private void FollowLink()
        {
            // var link = Item.Link;
            // TODO: 20210815 CJ: Open link in new tab!
        }

        private void HandleDragStarted(DragEventArgs e) => Board.DragAndDropService.HandleItemDragStarted(this);

        private void HandleDragEnded(DragEventArgs e)
        {
            Board.DragAndDropService.HandleItemDragEnded(this);
        }

        private void HandleDragEnter(DragEventArgs e) => Board.DragAndDropService.HandleItemDragEnter(this);

        private void HandleDragLeave(DragEventArgs e) => Board.DragAndDropService.HandleItemDragLeave(this);

        private void HandleDrop(DragEventArgs e) => Board.DragAndDropService.HandleItemDrop(this);
    }
}