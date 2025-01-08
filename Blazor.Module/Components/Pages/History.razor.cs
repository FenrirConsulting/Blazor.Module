using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.Module.Data.Repositories.Interfaces;
using Blazor.Module.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Blazor.Module.Components.Pages
{
    /// <summary>
    /// Represents the History page component.
    /// </summary>
    public partial class History : ComponentBase
    {
        #region Injected Services

        [Inject] private ISnackbar? _SnackBar { get; set; }
        [Inject] private ILogHelper? _Logger { get; set; }
        [Inject] private IAppConfiguration? _AppConfiguration { get; set; }
        [Inject] private IEnableDisableHistoryEntryRepository? _HistoryRepository { get; set; }
        [Inject] private NavigationManager _NavigationManager { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider _AuthenticationStateProvider { get; set; } = default!;

        #endregion

        #region Private Fields

        private string _selectedScope = "SingleAccount";
        private string _searchText = string.Empty;
        private List<HistoryViewModel> _items = new List<HistoryViewModel>();
        protected string UserDisplayName = string.Empty;

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Initializes the component.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitializeUserDisplayName();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the user display name from the authentication state.
        /// </summary>
        private async Task InitializeUserDisplayName()
        {
            var authState = await _AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                UserDisplayName = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets the label for the search box based on the selected scope.
        /// </summary>
        private string GetSearchBoxLabel()
        {
            return _selectedScope switch
            {
                "SingleAccount" => "SAM Account",
                "AllAccounts" => "Employee Number",
                "SubmittedByMe" => "SAM Account",
                "SubmittedByAccount" => "Submitted By SAM Account",
                _ => "Search",
            };
        }

        /// <summary>
        /// Handles the change of search scope.
        /// </summary>
        private void OnScopeChanged(string newScope)
        {
            _selectedScope = newScope;
            _searchText = _selectedScope == "SubmittedByMe" ? UserDisplayName : string.Empty;
        }

        /// <summary>
        /// Performs the search operation based on the selected scope.
        /// </summary>
        private async Task SearchAsync()
        {
            try
            {
                IEnumerable<EnableDisableHistoryEntry> historyEntries;

                _SnackBar?.Add($"Search completed, found {_items.Count} records.", Severity.Success);
            }
            catch (Exception ex)
            {
                HandleError("Error during search", ex);
            }
        }

        /// <summary>
        /// Refreshes all data in the history view.
        /// </summary>
        private async Task GetRefreshAllAsync()
        {
            try
            {
                var historyEntries = await _HistoryRepository!.GetAllAsync();
                var batchIds = historyEntries.Select(h => h.RequestID).ToList();

                _SnackBar?.Add("Data refreshed", Severity.Success);
            }
            catch (Exception ex)
            {
                HandleError("Error refreshing data", ex);
            }
        }

        /// <summary>
        /// Copies the table content to clipboard.
        /// </summary>
        private async Task CopyTable()
        {
            try
            {
                // TODO: Implement table copying logic
                await Task.CompletedTask;
                _SnackBar?.Add("Table copied to clipboard", Severity.Success);
            }
            catch (Exception ex)
            {
                HandleError("Error copying table", ex);
            }
        }

        /// <summary>
        /// Handles errors by displaying a snackbar message and logging the error.
        /// </summary>
        private void HandleError(string message, Exception ex)
        {
            _SnackBar?.Add(message, Severity.Error);
            _Logger?.LogError(message, ex);
        }

        /// <summary>
        /// Clears all data and resets the search fields.
        /// </summary>
        private void ClearData()
        {
            _items.Clear();
            _searchText = string.Empty;
            _selectedScope = "SingleAccount";
            StateHasChanged();
            _SnackBar?.Add("Data cleared", Severity.Info);
        }
        #endregion
    }
}