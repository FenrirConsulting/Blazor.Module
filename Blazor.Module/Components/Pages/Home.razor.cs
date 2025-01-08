using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.Infrastructure.Services;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.Module.Components.Pages
{
    /// <summary>
    /// Represents the home page component for the EnableDisable tool.
    /// </summary>
    public partial class Home : ComponentBase
    {
        #region Injected Services
        [Inject] private IAppConfiguration? _appConfiguration { get; set; }
        [Inject] private IAzureAdOptions? _azureConfiguration { get; set; }
        [Inject] private NavigationManager? _navigationManager { get; set; }
        [Inject] private ILogHelper? _Logger { get; set; }
        [Inject] private AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        #endregion

        #region Private Fields
        private bool IsLoading { get; set; } = false;

        #endregion

        #region Lifecycle Methods

        #endregion

        #region Private Methods

        #endregion
    }
}