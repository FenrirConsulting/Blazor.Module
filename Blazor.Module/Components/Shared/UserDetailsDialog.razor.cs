
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Reflection;

namespace Blazor.Module.Components.Shared
{
    public partial class UserDetailsDialog : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;


        private void Close() => MudDialog.Close();

    }
}
