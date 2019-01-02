using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuEve.Core.Entities
{
    /// <summary>
    /// Represents a corporation in the store.
    /// </summary>
    public class Corporation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Corporation"/> class.
        /// </summary>
        /// <param name="corporationId">The corporation identifier.</param>
        public Corporation(int corporationId)
        {
            CorporationId = corporationId;

            Characters = new List<Character>();
        }

        /// <summary>
        /// Gets or sets the corporation identifier.
        /// </summary>
        /// <value>
        /// The corporation identifier.
        /// </value>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CorporationId { get; set; }

        /// <summary>
        /// Gets or sets the alliance identifier.
        /// </summary>
        /// <value>
        /// The alliance identifier.
        /// </value>
        public int? AllianceId { get; set; }

        /// <summary>
        /// Gets or sets the alliance.
        /// </summary>
        /// <value>
        /// The alliance.
        /// </value>
        public Alliance Alliance { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [StringLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ticker.
        /// </summary>
        /// <value>
        /// The ticker.
        /// </value>
        [StringLength(255)]
        public string Ticker { get; set; }

        /// <summary>
        /// Gets or sets the date updated.
        /// </summary>
        /// <value>
        /// The date updated.
        /// </value>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the the date API response is cached until.
        /// </summary>
        /// <value>
        /// The cached until.
        /// </value>
        public DateTime CachedUntil { get; set; }

        /// <summary>
        /// Gets or sets the e tag.
        /// </summary>
        /// <value>
        /// The e tag.
        /// </value>
        [StringLength(255)]
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the characters.
        /// </summary>
        /// <value>
        /// The characters.
        /// </value>
        public ICollection<Character> Characters { get; set; }
    }
}
