using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;

namespace Blazor.Module.Components.Shared
{
    public partial class ResponseDetailsDialog : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;

        [Parameter] public string RawResponse { get; set; } = string.Empty;
        [Parameter] public bool SuccessResponse { get; set; }

        [Inject] private ISnackbar? _snackbar { get; set; }

        private void Cancel() => MudDialog.Cancel();

        private void AddToTable() => MudDialog.Close(DialogResult.Ok(true));

        private string FormatJson(string json)
        {
            try
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
                return JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                _snackbar!.Add("Error formatting JSON response", Severity.Warning);
                return json;
            }
        }
    }
}
