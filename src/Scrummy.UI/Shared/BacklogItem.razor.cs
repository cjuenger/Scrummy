using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class BacklogItem
    {
        [Parameter] 
        public Item Item { get; set; }
    }
}