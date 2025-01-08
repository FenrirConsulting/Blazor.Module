using System;
using System.ComponentModel.DataAnnotations;

namespace Blazor.Module.Models
{
    /// <summary>
    /// Represents a combined view of history entry and directory action data.
    /// </summary>
    public class HistoryViewModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the unique request identifier.
        /// </summary>
        [MaxLength(100)]
        public string? RequestID { get; set; }

        /// <summary>
        /// Gets or sets the user who submitted the entry.
        /// </summary>
        [MaxLength(50)]
        public string? SubmittedBy { get; set; }

        /// <summary>
        /// Gets or sets the completion status of the history entry.
        /// </summary>
        [MaxLength(20)]
        public string? Completed { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the action.
        /// </summary>
        [MaxLength(30)]
        public string? WhenCreated { get; set; }

        /// <summary>
        /// Gets or sets the result of the action.
        /// </summary>
        [MaxLength(20)]
        public string? ActionResult { get; set; }

        /// <summary>
        /// Gets or sets the source domain.
        /// </summary>
        [MaxLength(200)]
        public string? SourceDomain { get; set; }

        /// <summary>
        /// Gets or sets the disabled status.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the source SAM account name.
        /// </summary>
        [MaxLength(200)]
        public string? SourceSamAccountName { get; set; }

        /// <summary>
        /// Gets or sets the source original Organizational Unit.
        /// </summary>
        [MaxLength(200)]
        public string? SourceOriginalOU { get; set; }

        /// <summary>
        /// Gets or sets the Extension Attribute 8 value.
        /// </summary>
        [MaxLength(200)]
        public string? ExtensionAttribute8 { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the source manager.
        /// </summary>
        [MaxLength(200)]
        public string? SourceManager { get; set; }

        /// <summary>
        /// Gets or sets the source member of groups.
        /// </summary>
        [MaxLength(200)]
        public string? SourceMemberOf { get; set; }

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        [MaxLength(50)]
        public string? EmployeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the msDBUseDefaults value.
        /// </summary>
        [MaxLength(200)]
        public string? MsDBUseDefaults { get; set; }

        /// <summary>
        /// Gets or sets the msExchHideFromAddressList value.
        /// </summary>
        [MaxLength(1000)]
        public string? MsExchHideFromAddressList { get; set; }

        #endregion
    }
}