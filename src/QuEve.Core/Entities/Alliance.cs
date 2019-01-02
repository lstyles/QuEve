using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuEve.Core.Entities
{
    /// <summary>
    /// Represents an alliance in the store.
    /// </summary>
    public class Alliance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alliance"/> class.
        /// </summary>
        /// <param name="allianceId">The alliance identifier.</param>
        public Alliance(int allianceId)
        {
            AllianceId = allianceId;

            Corporations = new List<Corporation>();
        }

        /// <summary>
        /// Gets or sets the alliance identifier.
        /// </summary>
        /// <value>
        /// The alliance identifier.
        /// </value>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AllianceId { get; set; }

        /// <summary>
        /// Gets or sets the the date API response is cached until.
        /// </summary>
        /// <value>
        /// The cached until.
        /// </value>
        public DateTime CachedUntil { get; set; }

        /// <summary>
        /// Gets or sets the date updated.
        /// </summary>
        /// <value>
        /// The date updated.
        /// </value>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the e tag.
        /// </summary>
        /// <value>
        /// The e tag.
        /// </value>
        [StringLength(255)]
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the alliance name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [StringLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the alliance ticker.
        /// </summary>
        /// <value>
        /// The ticker.
        /// </value>
        [StringLength(255)]
        public string Ticker { get; set; }

        /// <summary>
        /// Gets or sets the corporations that are members of this alliance.
        /// </summary>
        /// <value>
        /// Alliance member corproations.
        /// </value>
        public ICollection<Corporation> Corporations { get; }
    }
}