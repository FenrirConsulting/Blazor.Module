
namespace Blazor.Module.Models
{
    /// <summary>
    /// Adapter class to convert EnableTableItem to IRequestBuildItem.
    /// </summary>
    public class EnableTableItemAdapter 
    {
        /// <summary>
        /// Gets the status of the item.
        /// </summary>
        public string Status { get; }

        /// <summary>
        /// Gets the comment associated with the item.
        /// </summary>
        public string Comment { get; }

        /// <summary>
        /// Gets the action to be performed on the item.
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Gets the scope of the item.
        /// </summary>
        public string Scope { get; }

        /// <summary>
        /// Gets the domain of the item.
        /// </summary>
        public string Domain { get; }

        /// <summary>
        /// Gets the SAM account of the item.
        /// </summary>
        public string SamAccount { get; }

        /// <summary>
        /// Gets the disable comment for the item.
        /// </summary>
        public string DisableComment { get; }


        /// <summary>
        /// Initializes a new instance of the EnableTableItemAdapter class.
        /// </summary>
        /// <param name="item">The EnableTableItem to adapt.</param>
        public EnableTableItemAdapter(EnableTableItem item)
        {
            Status = item.Status;
            Comment = item.Comment;
            Action = item.Action;
            Scope = item.Scope;
            Domain = item.Domain;
            SamAccount = item.SamAccount;
            DisableComment = item.DisableComment;
        }
    }
}
