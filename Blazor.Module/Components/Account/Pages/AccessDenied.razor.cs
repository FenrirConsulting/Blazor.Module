using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Navigation;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Module.Components.Account.Pages
{
    /// <summary>
    /// Represents the Access Denied page component.
    /// </summary>
    public partial class AccessDenied : ComponentBase
    {
        #region Injected Services
        [Inject] private NavigationManager? _navigationManager { get; set; }
        [Inject] private IAppConfiguration? _appConfiguration { get; set; }
        [Inject] private ILogHelper _logger { get; set; } = default!;
        #endregion

        #region Public Methods
        /// <summary>
        /// Navigates the user to the login page with a return URL.
        /// </summary>
        private void Login()
        {
            try
            {
                var returnUrl = Uri.EscapeDataString(_navigationManager!.Uri);
                _navigationManager.NavigateTo($"{_appConfiguration!.BaseUrl}account/login?returnUrl={returnUrl}", forceLoad: false);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while navigating to login page", ex);
            }
        }
        #endregion
    }
}