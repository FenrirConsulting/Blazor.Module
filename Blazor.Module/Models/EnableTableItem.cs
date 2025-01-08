
namespace Blazor.Module.Models
{
    /// <summary>
    /// Represents an item in the enable/disable operation table.
    /// </summary>
    public class EnableTableItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the status of the operation.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the comment associated with the operation.
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the action to be performed (Enable/Disable).
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the scope of the operation (SingleAccount/AllAccounts).
        /// </summary>
        public string Scope { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the domain of the account.
        /// </summary>
        public string Domain { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the SAM account name or employee number.
        /// </summary>
        public string SamAccount { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the comment for disable operations.
        /// </summary>
        public string DisableComment { get; set; } = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableTableItem"/> class.
        /// </summary>
        public EnableTableItem()
        {
            // Default constructor
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableTableItem"/> class with specified values.
        /// </summary>
        /// <param name="status">The status of the operation.</param>
        /// <param name="comment">The comment associated with the operation.</param>
        /// <param name="action">The action to be performed.</param>
        /// <param name="scope">The scope of the operation.</param>
        /// <param name="domain">The domain of the account.</param>
        /// <param name="samAccount">The SAM account name or employee number.</param>
        /// <param name="disableComment">The comment for disable operations.</param>
        public EnableTableItem(string status, string comment, string action, string scope, string domain, string samAccount, string disableComment)
        {
            Status = status;
            Comment = comment;
            Action = action;
            Scope = scope;
            Domain = domain;
            SamAccount = samAccount;
            DisableComment = disableComment;
        }
        #endregion
    }
}