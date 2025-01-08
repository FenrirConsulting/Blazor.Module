using System;

namespace Blazor.Module.Models
{
    /// <summary>
    /// Represents a server configuration for disable operations.
    /// </summary>
    public class DisableServer
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the server address.
        /// </summary>
        public string Server { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the port number for server connections.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the Organizational Unit (OU) for terminated accounts.
        /// </summary>
        public string TermOU { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DisableServer class.
        /// </summary>
        public DisableServer()
        {
            // Default constructor
        }

        #endregion
    }
}
