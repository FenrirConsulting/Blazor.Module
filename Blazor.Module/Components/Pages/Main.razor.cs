using Blazor.Module.Components.Shared;
using Blazor.Module.Models;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Infrastructure.Authentication.Interfaces;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Identity.Client;
using MudBlazor;
using System.Text.Json;
using Blazor.RCL.UIComponents.Components;
using Microsoft.JSInterop;
using Blazor.Module.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Module.Components.Pages
{
    /// <summary>
    /// Represents the main component for the Enable-Disable page.
    /// </summary>
    public partial class Main : ComponentBase, IDisposable
    {
        #region Dependency Injection

        [Inject] private ISnackbar? _snackbar { get; set; }
        [Inject] private ILogHelper? _logger { get; set; }
        [Inject] private IAppConfiguration? _appConfiguration { get; set; }
        [Inject] private IAzureAdOptions? _azureAdOptions { get; set; }
        [Inject] private IJSRuntime? _jsRuntime { get; set; }
        [Inject] private ILdapAuthenticationService? _ldapAuthenticationService { get; set; }
        [Inject] private IToolsConfigurationRepository? _toolsConfigurationRepository { get; set; }
        [Inject] private LdapServerList? _ldapServerList { get; set; }
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private IEnableDisableHistoryEntryRepository? _historyRepository { get; set; }
        [Inject] protected AuthenticationStateProvider _AuthenticationStateProvider { get; set; } = default!;

        #endregion

        #region Private Fields

        private string corpOmnicare = string.Empty;
        private string password = string.Empty;
        private string selectedInput = "Manual";
        private string selectedAction = "Disable";
        private string selectedScope = "SingleAccount";
        private string selectedDomain = string.Empty;
        private string samAccount = string.Empty;
        private string disableComment = string.Empty;
        private string fileName = string.Empty;
        private string selectedCriteria = "SAMAccount";
        private List<EnableTableItem> items = new List<EnableTableItem>();
        private List<DisableServer> disableServers = new List<DisableServer>();
        private DisableServer? omnicareDisableServer;
        private bool omnicareValidated = false;
        private bool isSingleAccountEnabled = true;
        private string omnicareValidateGroup = string.Empty;
        private OmnicareServer omnicareServer = new OmnicareServer();
        private List<string> disableComments = new List<string>();
        private Dictionary<string, string> appConfigurations = new Dictionary<string, string>();
        private bool isLoadingValidation = false;
        private bool isLoadingRequest = false;

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Initializes the component and loads initial data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadInitialData();
        }

        /// <summary>
        /// Performs cleanup of resources.
        /// </summary>
        public void Dispose()
        {
            // Implement resource cleanup if necessary
        }

        #endregion

        #region Data Loading and Configuration

        /// <summary>
        /// Loads initial data for the component.
        /// </summary>
        private async Task LoadInitialData()
        {
            try
            {
                await LoadConfigurations();
                LoadDisableServers();
                LoadDisableComments();
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                _snackbar?.Add("Error loading initial data", Severity.Error);
                _logger?.LogError("Error loading initial data", ex);
            }
        }

        /// <summary>
        /// Loads configurations from the repository.
        /// </summary>
        private async Task LoadConfigurations()
        {
            var configurations = await _toolsConfigurationRepository!.GetAllByApplicationAsync(_appConfiguration!.AppName);
            appConfigurations = configurations
                .Where(c => c.Setting != null)
                .ToDictionary(c => c.Setting!, c => c.Value ?? string.Empty);

            omnicareValidateGroup = GetConfigurationValue("OmnicareValidateGroup");
            if (string.IsNullOrEmpty(omnicareValidateGroup))
            {
                _snackbar?.Add("Could not load ValidateGroup", Severity.Error);
                _logger?.LogMessage("OmnicareValidateGroup is null or empty");
            }
        }

        /// <summary>
        /// Loads the DisableServers configuration.
        /// </summary>
        private void LoadDisableServers()
        {
            var disableServersConfig = GetConfigurationValue("EnableDisableServers");
            if (!string.IsNullOrEmpty(disableServersConfig))
            {
                try
                {
                    disableServers = JsonSerializer.Deserialize<List<DisableServer>>(disableServersConfig, new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip
                    }) ?? new List<DisableServer>();
                }
                catch (JsonException jsonEx)
                {
                    HandleJsonException(jsonEx, "DisableServers");
                }
            }
            else
            {
                _snackbar?.Add("Could not load DisableServers", Severity.Error);
                _logger?.LogMessage("EnableDisableServers configuration is null or empty");
            }
        }

        /// <summary>
        /// Loads the DisableComments configuration.
        /// </summary>
        private void LoadDisableComments()
        {
            var disableCommentsConfig = GetConfigurationValue("DisableComments");
            disableComments = new List<string>();

            if (!string.IsNullOrEmpty(disableCommentsConfig))
            {
                try
                {
                    var loadedComments = JsonSerializer.Deserialize<List<string>>(disableCommentsConfig, new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip
                    });

                    if (loadedComments != null)
                    {
                        disableComments.AddRange(loadedComments);
                    }
                }
                catch (JsonException jsonEx)
                {
                    HandleJsonException(jsonEx, "DisableComments");
                }
            }
            else
            {
                _snackbar?.Add("Could not load DisableComments", Severity.Error);
                _logger?.LogMessage("DisableComments configuration is null or empty");
            }

            if (!disableComments.Contains("Other"))
            {
                disableComments.Add("Other");
            }
        }

        #endregion

        #region Authentication and API Calls

        /// <summary>
        /// Authenticates the Omnicare user.
        /// </summary>
        private async Task AuthenticateAsync()
        {
            if (string.IsNullOrEmpty(omnicareValidateGroup))
            {
                _snackbar?.Add("ValidateGroup not loaded. Please try again later.", Severity.Error);
                return;
            }

            try
            {
                var (isAuthenticated, groups) = await _ldapAuthenticationService!.ValidateCredentialsAndFetchGroupsAsync(
                    omnicareServer.Name,
                    omnicareServer.Server,
                    omnicareServer.Port,
                    omnicareServer.SearchBase,
                    omnicareServer.AdminSearchBase,
                    corpOmnicare,
                    password
                );

                if (!isAuthenticated)
                {
                    _snackbar?.Add("Invalid credentials", Severity.Warning);
                    omnicareValidated = false;
                }
                else
                {
                    omnicareValidated = groups.Contains(omnicareValidateGroup);
                    _snackbar?.Add(omnicareValidated ? "Omnicare Validated" : "NO Omnicare Admin Account Found",
                                  omnicareValidated ? Severity.Success : Severity.Warning);
                }

                isSingleAccountEnabled = true;
            }
            catch (Exception ex)
            {
                _snackbar?.Add("Authentication failed", Severity.Error);
                _logger?.LogError("Authentication error", ex);
                omnicareValidated = false;
            }
        }

        /// <summary>
        /// Validates the form inputs.
        /// </summary>
        private async Task ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(samAccount))
            {
                _snackbar?.Add("Please fill in all required fields", Severity.Warning);
                return;
            }

            isLoadingValidation = true;
            StateHasChanged();

            try
            {

                try
                {
                    
                }
                catch (Exception ex)
                {
                    _snackbar?.Add("Error calling AD User Details API", Severity.Error);
                    _logger?.LogError("Error calling AD User Details API", ex);
                }
            }
            finally
            {
                isLoadingValidation = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Executes the enable/disable action.
        /// </summary>
        private async Task ExecuteActionAsync()
        {
            try
            {
                if (items.Count == 0)
                {
                    _snackbar?.Add($"No items to process", Severity.Warning);
                    return;
                }


                isLoadingRequest = true;
                StateHasChanged();

                string batchId = string.Empty;

                if (selectedAction == "Disable")
                {
                  
                }

                else if (selectedAction == "Enable")
                {
                   
                }

            }
            catch (Exception ex)
            {
                _snackbar?.Add($"Error executing {selectedAction} action", Severity.Error);
                _logger?.LogError($"Error executing {selectedAction} action", ex);
            }
            finally
            {
                isLoadingRequest = false;
                StateHasChanged();
            }
        }


        /// <summary>
        /// Generates the disable request payload and displays it in a dialog.
        /// </summary>
        private async Task GenerateRequestPayload()
        {
            try
            {
                if (items.Count == 0)
                {
                    _snackbar?.Add($"No items to process", Severity.Warning);
                    return;
                }

                List<object> allRequests = new List<object>();
                string batchId = string.Empty;

                if (selectedAction == "Disable")
                {

                }
                else if (selectedAction == "Enable")
                {

                }

                var testData = new
                {
                    BatchId = batchId,
                    RequestCount = allRequests.Count,
                    Requests = allRequests
                };

                var jsonContent = JsonSerializer.Serialize(testData, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var parameters = new DialogParameters { { "JsonContent", jsonContent } };
                DialogService.Show<JsonDialog>("Test Request Payload with BatchID", parameters);

            }
            catch (Exception ex)
            {
                _snackbar?.Add($"Error generating test payload", Severity.Error);
                _logger?.LogError($"Error generating test payload", ex);
            }
        }

        #endregion

        #region UI Interaction Methods

        /// <summary>
        /// Resets the form to its initial state.
        /// </summary>
        private void ResetForm()
        {
            corpOmnicare = string.Empty;
            password = string.Empty;
            selectedInput = "Manual";
            selectedAction = "Disable";
            selectedScope = "SingleAccount";
            selectedDomain = string.Empty;
            samAccount = string.Empty;
            disableComment = string.Empty;
            fileName = string.Empty;
            items.Clear();
        }

        /// <summary>
        /// Gets the text for the action button based on the selected scope.
        /// </summary>
        /// <returns>The button text.</returns>
        private string GetButtonText()
        {
            return selectedScope == "AllAccounts" ? "Enable / Disable" : selectedAction;
        }

        /// <summary>
        /// Handles the file input change event.
        /// </summary>
        /// <param name="e">The input file change event arguments.</param>
        private async Task OnInputFileChanged(InputFileChangeEventArgs e)
        {
            try
            {
                var file = e.File;
                if (file != null)
                {
                    fileName = file.Name;
                    // TODO: Process file contents
                    items.Add(new EnableTableItem
                    {
                        Status = "Pending",
                        Comment = "Imported from file",
                        Action = selectedAction,
                        Scope = selectedScope,
                        Domain = selectedDomain,
                        SamAccount = "ImportedAccount",
                        DisableComment = string.Empty
                    });
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception ex)
            {
                _snackbar?.Add("Error processing file", Severity.Error);
                _logger?.LogError("Error processing file", ex);
            }
        }

        /// <summary>
        /// Handles the file import process.
        /// </summary>
        private async Task ImportFile()
        {
            if (string.IsNullOrEmpty(fileName))
            {
                _snackbar?.Add("Please select a file first", Severity.Warning);
                return;
            }

            try
            {
                // TODO: Implement file import logic here
                await Task.Delay(1000); // Simulating file processing
                _snackbar?.Add("File imported successfully", Severity.Success);
            }
            catch (Exception ex)
            {
                _snackbar?.Add("Error importing file", Severity.Error);
                _logger?.LogError("Error importing file", ex);
            }
        }

        /// <summary>
        /// Copies the table content to clipboard.
        /// </summary>
        private async Task CopyTable()
        {
            try
            {
                var tableContent = string.Join("\n", items.ConvertAll(item =>
                    $"{item.Status}\t{item.Comment}\t{item.Action}\t{item.Scope}\t{item.Domain}\t{item.SamAccount}\t{item.DisableComment}"));

                await _jsRuntime!.InvokeVoidAsync("navigator.clipboard.writeText", tableContent);
                _snackbar?.Add("Table copied to clipboard", Severity.Success);
            }
            catch (Exception ex)
            {
                _snackbar?.Add("Error copying table", Severity.Error);
                _logger?.LogError("Error copying table", ex);
            }
        }

        /// <summary>
        /// Deletes an item from the list after confirmation.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        private async void DeleteItem(EnableTableItem item)
        {
            bool? result = await DialogService.ShowMessageBox(
                "Warning",
                "Remove this item from the list? Are you sure?",
                yesText: "Yes", cancelText: "Cancel");

            if (result == true)
            {
                items.Remove(item);
                StateHasChanged();
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Retrieves a configuration value by key.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value if found, otherwise an empty string.</returns>
        private string GetConfigurationValue(string key)
        {
            if (appConfigurations.TryGetValue(key, out var value))
            {
                return value;
            }
            _snackbar?.Add($"Configuration '{key}' not found", Severity.Warning);
            _logger?.LogMessage($"Configuration '{key}' not found");
            return string.Empty;
        }

        /// <summary>
        /// Handles JSON deserialization exceptions.
        /// </summary>
        /// <param name="jsonEx">The JSON exception.</param>
        /// <param name="configName">The name of the configuration causing the exception.</param>
        private void HandleJsonException(JsonException jsonEx, string configName)
        {
            _snackbar?.Add($"Error parsing {configName} configuration: {jsonEx.Message}", Severity.Error);
            _logger?.LogError($"JSON parsing error in {configName} configuration: {jsonEx.Message}", jsonEx);
        }

        /// <summary>
        /// Builds the JSON content for the enable/disable request.
        /// </summary>
        /// <returns>The JSON content as a string, or null if invalid.</returns>
        private string BuildJsonContent()
        {
         

            if (selectedAction == "Disable")
            {
                return null!;
            }
            else if (selectedAction == "Enable")
            {
                return null!;
            }
            else
            {
                _snackbar?.Add("Invalid action selected", Severity.Warning);
                return null!;
            }
        }
        #endregion
    }
}