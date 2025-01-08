using Blazor.Module.Data.Repositories.Interfaces;
using Blazor.Module.Models;
using Blazor.RCL.NLog.LogService.Interface;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Module.Data.Repositories
{
    /// <summary>
    /// Repository implementation for history entry operations using Entity Framework Core.
    /// </summary>
    public class EnableDisableHistoryEntryRepository : IEnableDisableHistoryEntryRepository
    {
        #region Fields

        private readonly IDbContextFactory<ScopedDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the HistoryEntryRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public EnableDisableHistoryEntryRepository(IDbContextFactory<ScopedDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async Task<IEnumerable<EnableDisableHistoryEntry>> GetAllAsync()
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                return await context.EnableDisableHistoryEntries.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while retrieving all history entries", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EnableDisableHistoryEntry>> GetByBatchIdsAsync(IEnumerable<string> batchIds)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                return await context.EnableDisableHistoryEntries
                .Where(e => batchIds.Contains(e.BatchID))
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while retrieving batch ID History entries.", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EnableDisableHistoryEntry>> GetByRequestIdsAsync(IEnumerable<string> requestIds)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                return await context.EnableDisableHistoryEntries
                .Where(e => requestIds.Contains(e.RequestID))
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while retrieving request ID History entries.", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EnableDisableHistoryEntry>> GetBySubmittedByAsync(string submittedBy)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                return await context.EnableDisableHistoryEntries
                    .Where(h => h.SubmittedBy == submittedBy)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while retrieving history entries submitted by: {submittedBy}", ex);
                throw;
            }
        }

        public async Task AddAsync(EnableDisableHistoryEntry entry)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                await context.EnableDisableHistoryEntries.AddAsync(entry);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while adding a history entry", ex);
                throw;
            }
        }
        #endregion
    }
}
