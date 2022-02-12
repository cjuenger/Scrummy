using Microsoft.AspNetCore.Components;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class BacklogItemButton
    {
        [Parameter] public Item Item { get; set; }

        private void FollowLink()
        {
            var link = Item.Link;
            // TODO: 20210815 CJ: Open link in new tab!
        }
    }
}