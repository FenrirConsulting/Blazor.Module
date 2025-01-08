using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Blazor.RCL.Domain.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Blazor.RCL.Infrastructure.Data;
using Blazor.Module.Models;

namespace Blazor.Module.Data
{
    public class ScopedDbContext : DbContext
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AppDbContext class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ScopedDbContext(DbContextOptions<ScopedDbContext> options) : base(options) { }

        #endregion

        #region DbSet Properties
        /// <summary>
        /// Gets or sets the DbSet for HistoryEntries.
        /// </summary>
        public DbSet<EnableDisableHistoryEntry> EnableDisableHistoryEntries { get; set; } = null!;
        #endregion
    }
}
