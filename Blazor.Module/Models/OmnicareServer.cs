using System;

namespace Blazor.Module.Models
{
    public class OmnicareServer
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the LDAP server.
        /// </summary>
        public string Name { get; set; } = "Omnicare";

        /// <summary>
        /// Gets or sets the LDAP server address.
        /// </summary>
        public string Server { get; set; } = "corp.omnicare.com";

        /// <summary>
        /// Gets or sets the port number for LDAP connections.
        /// </summary>
        public string Port { get; set; } = "389";

        /// <summary>
        /// Gets or sets the search base for LDAP queries.
        /// </summary>
        public string SearchBase { get; set; } = "OU=ADM Accounts,OU=1-Corporate Offices,OU=1-Omnicare,DC=corp,DC=omnicare,DC=com";

        /// <summary>
        /// Gets or sets the admin search base for LDAP queries.
        /// </summary>
        public string AdminSearchBase { get; set; } = "OU=ADM Accounts,OU=1-Corporate Offices,OU=1-Omnicare,DC=corp,DC=omnicare,DC=com";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the OmnicareServer class with default values.
        /// </summary>
        public OmnicareServer()
        {
            // Default constructor
        }

        #endregion
    }
}
