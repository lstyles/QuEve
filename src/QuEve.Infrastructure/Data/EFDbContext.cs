using Microsoft.EntityFrameworkCore;
using QuEve.Core.Entities;

namespace QuEve.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework DB Context
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public class EFDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EFDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public EFDbContext(DbContextOptions<EFDbContext> options)
            : base(options)
        { }

        /// <summary>
        /// Gets or sets the accounts.
        /// </summary>
        /// <value>
        /// The accounts.
        /// </value>
        public DbSet<Account> Accounts { get; set; }

        /// <summary>
        /// Gets or sets the alliances.
        /// </summary>
        /// <value>
        /// The alliances.
        /// </value>
        public DbSet<Alliance> Alliances { get; set; }

        /// <summary>
        /// Gets or sets the characters.
        /// </summary>
        /// <value>
        /// The characters.
        /// </value>
        public DbSet<Character> Characters { get; set; }

        /// <summary>
        /// Gets or sets the corporations.
        /// </summary>
        /// <value>
        /// The corporations.
        /// </value>
        public DbSet<Corporation> Corporations { get; set; }

        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Account
            builder.Entity<Account>(b =>
            {
                b.HasMany(a => a.Characters).WithOne(c => c.Account).OnDelete(DeleteBehavior.Cascade);
            });

            // Alliance
            builder.Entity<Alliance>(b =>
            {
                b.HasMany(a => a.Corporations).WithOne(c => c.Alliance).OnDelete(DeleteBehavior.Restrict);
            });

            // Corporation
            builder.Entity<Corporation>(b =>
            {
                b.HasMany(c => c.Characters).WithOne(c => c.Corporation).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
