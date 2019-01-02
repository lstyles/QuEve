using System.Collections.Generic;

namespace QuEve.Infrastructure.Configuration
{
    /// <summary>
    /// Security Options
    /// </summary>
    public class SecurityOptions
    {
        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public List<string> Scopes { get; set; }
    }
}
