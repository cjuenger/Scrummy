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
        [Parameter] public Item Item { get; set; }
        
        private string Id { get; }
        
        [CascadingParameter] protected BoardComponent Board { get; set; }
        
        [Inject] private IJSRuntime JsRuntime { get; set; }
        
        private DotNetObjectReference<ItemComponent> _objRef;

        public ItemComponent() => Id = Guid.NewGuid().ToString();
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _objRef = DotNetObjectReference.Create(this);
                await JsRuntime.InvokeVoidAsync("addOnDragStartEventHandler", _objRef, Id);
            }
            
            await base.OnAfterRenderAsync(firstRender);
        }

        private void FollowLink()
        {
            // var link = Item.Link;
            // TODO: 20210815 CJ: Open link in new tab!
        }
        
        [JSInvokable]
        public void HandleDragStarted(DragEventArgs e) => Board.DragAndDropService.HandleDragStarted(Item, e);

        private void HandleDragEnded(DragEventArgs e) => Board.DragAndDropService.HandleDragEnded(Item, e.ScreenX, e.ScreenY);

        private void HandleDragEnter(DragEventArgs e) => Board.DragAndDropService.HandleDragEnter(Item, e.ScreenX, e.ScreenY);

        private void HandleDragLeave(DragEventArgs e) => Board.DragAndDropService.HandleDragLeave(Item);
    }
}