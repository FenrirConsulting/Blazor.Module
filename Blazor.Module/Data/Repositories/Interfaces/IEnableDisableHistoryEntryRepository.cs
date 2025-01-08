using Blazor.Module.Models;

namespace Blazor.Module.Data.Repositories.Interfaces
{
    /// <summary>
    /// Defines the interface for history entry repository operations.
    /// </summary>
    public interface IEnableDisableHistoryEntryRepository
    {
        /// <summary>
        /// Retrieves all history entries asynchronously.
        /// </summary>
        /// <returns>A collection of all history entries.</returns>
        Task<IEnumerable<EnableDisableHistoryEntry>> GetAllAsync();

        /// <summary>
        /// Retrieves a history entry by its batch ID asynchronously.
        /// </summary>
        /// <param name="batchIds">The batch ID of the history entry to retrieve.</param>
        /// <returns>The history entry with the specified batch ID, or null if not found.</returns>
        Task<IEnumerable<EnableDisableHistoryEntry>> GetByBatchIdsAsync(IEnumerable<string> batchIds);

        /// <summary>
        /// Retrieves a history entry by its request ID asynchronously.
        /// </summary>
        /// <param name="requestIds">The request ID of the history entry to retrieve.</param>
        /// <returns>The history entry with the specified request ID, or null if not found.</returns>
        Task<IEnumerable<EnableDisableHistoryEntry>> GetByRequestIdsAsync(IEnumerable<string> requestIds);

        /// <summary>
        /// Retrieves history entries by the user who submitted them asynchronously.
        /// </summary>
        /// <param name="submittedBy">The user who submitted the entries.</param>
        /// <returns>A collection of history entries submitted by the specified user.</returns>
        Task<IEnumerable<EnableDisableHistoryEntry>> GetBySubmittedByAsync(string submittedBy);

        /// <summary>
        /// Adds new he history entry asynchronously.
        /// </summary>
        /// <param name="entry">The entry object paramter
        Task AddAsync(EnableDisableHistoryEntry entry);
    }
}
