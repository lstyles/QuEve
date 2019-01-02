using System;
using System.Collections.Generic;

namespace QuEve.Core.Entities
{
    /// <summary>
    /// Represents an account
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account()
        {
            DateCreated = DateTime.UtcNow;

            Characters = new List<Character>();
        }

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        /// <value>
        /// The date created.
        /// </value>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets the characters.
        /// </summary>
        /// <value>
        /// The characters.
        /// </value>
        public ICollection<Character> Characters { get; }
    }
}
