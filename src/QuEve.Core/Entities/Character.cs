using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuEve.Core.Entities
{
    /// <summary>
    /// Represents a character in the store.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class.
        /// </summary>
        /// <param name="characterId">The character identifier.</param>
        public Character(int characterId, int accountId)
        {
            AccountId = accountId;
            CharacterId = characterId;
        }

        /// <summary>
        /// Gets or sets the character identifier.
        /// </summary>
        /// <value>
        /// The character identifier.
        /// </value>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CharacterId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [StringLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the account id
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the account
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// Gets or sets the corporation identifier.
        /// </summary>
        /// <value>
        /// The corporation identifier.
        /// </value>
        public int CorporationId { get; set; }

        /// <summary>
        /// Gets or sets the corporation.
        /// </summary>
        /// <value>
        /// The corporation.
        /// </value>
        public Corporation Corporation { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the access token expiry.
        /// </summary>
        /// <value>
        /// The access token expiry.
        /// </value>
        public DateTime? AccessTokenExpiry { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        [StringLength(255)]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the e tag.
        /// </summary>
        /// <value>
        /// The e tag.
        /// </value>
        [StringLength(255)]
        public string ETag { get; set; }

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
    }
}
