using System;
using System.ComponentModel.DataAnnotations;

namespace Blazor.Module.Models
{
    /// <summary>
    /// Represents a history entry in the system.
    /// </summary>
    public class EnableDisableHistoryEntry
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the unique batch identifier.
        /// </summary>
        [Key]
        [MaxLength(100)]
        public string? BatchID { get; set; }

        /// <summary>
        /// Gets or sets the unique request identifier.
        /// </summary>
        [MaxLength(100)]
        public string? RequestID { get; set; }

        /// <summary>
        /// Gets or sets the unique ApplicationName identifier.
        /// </summary>
        [MaxLength(100)]
        public string? ApplicationNameID { get; set; }

        /// <summary>
        /// Gets or sets the user who submitted the entry.
        /// </summary>
        [MaxLength(50)]
        public string? SubmittedBy { get; set; }

        /// <summary>
        /// Gets or sets audit datetime
        /// </summary>
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// Gets or sets the Response JSON Data
        /// </summary>
        public string? ResponseData { get; set; }

        /// <summary>
        /// Gets or sets the Request JSON Data
        /// </summary>
        public string? ItemData { get; set; }

        /// <summary>
        /// Gets or sets the type of action
        /// </summary>
        [MaxLength(50)]
        public string? Action { get; set; }
        #endregion
    }
}