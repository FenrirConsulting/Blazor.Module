using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Identity.Client;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.Domain.Entities.Configuration;

namespace Blazor.Module.Components.Pages
{
    /// <summary>
    /// Represents the admin page component for managing tool configurations.
    /// </summary>
    public partial class Admin : ComponentBase
    {
        #region Injected Services
        [Inject] private IToolsConfigurationRepository? _toolsRepository { get; set; }
        [Inject] private IAppConfiguration? _appConfiguration { get; set; }
        [Inject] private ISnackbar? _snackbar { get; set; }
        [Inject] private ILogHelper? _logger { get; set; }
        [Inject] private IAzureAdOptions? _azureAdOptionsService { get; set; }
        #endregion

        #region Private Fields
        private List<ToolsConfiguration>? _toolConfigurations;
        private ToolsConfiguration? _editingItem;
        private bool _isDialogVisible;
        private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        private bool _isNewItem;

        private string SecretValue = "";
        #endregion

        #region Lifecycle Methods
        protected override async Task OnInitializedAsync()
        {
            await LoadToolConfigurations();
        }
        #endregion

        #region Private Methods
        private async Task LoadToolConfigurations()
        {
            try
            {
                var configurations = await _toolsRepository!.GetAllByApplicationAsync(_appConfiguration!.AppName);
                _toolConfigurations = configurations.ToList();
            }
            catch (Exception ex)
            {
                _snackbar!.Add("Error fetching data", Severity.Error);
                _logger!.LogError("Error fetching data", ex);
            }
        }

        private void OpenEditor(ToolsConfiguration? item)
        {
            _isNewItem = item == null;
            _editingItem = _isNewItem ? new ToolsConfiguration { Application = _appConfiguration!.AppName } : item;
            _isDialogVisible = true;
        }

        private void CancelEdit()
        {
            _editingItem = null;
            _isDialogVisible = false;
            _isNewItem = false;
        }

        private async Task SaveChanges()
        {
            try
            {
                await _toolsRepository!.SetAsync(_editingItem!);
                _snackbar!.Add(_isNewItem ? "Item added successfully" : "Item updated successfully", Severity.Success);
                await LoadToolConfigurations(); // Refresh data
            }
            catch (Exception ex)
            {
                _snackbar!.Add($"Error {(_isNewItem ? "adding" : "updating")} item: {ex.Message}", Severity.Error);
                _logger!.LogError($"Error {(_isNewItem ? "adding" : "updating")} item:", ex);
            }
            finally
            {
                _editingItem = null;
                _isDialogVisible = false;
                _isNewItem = false;
            }
        }

        private async Task RemoveItem(long id)
        {
            try
            {
                await _toolsRepository!.RemoveAsync(new ToolsConfiguration { Id = id });
                _snackbar!.Add("Item removed successfully", Severity.Success);
                await LoadToolConfigurations(); // Refresh data
            }
            catch (Exception ex)
            {
                _snackbar!.Add($"Error removing item: {ex.Message}", Severity.Error);
                _logger!.LogError("Error removing item:", ex);
            }
        }

        

        
        

        #endregion
    }
}
