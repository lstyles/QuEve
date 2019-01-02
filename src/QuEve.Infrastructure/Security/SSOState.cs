using Newtonsoft.Json;
using System;
using System.Text;

namespace QuEve.Infrastructure.Security
{
    /// <summary>
    /// SSO State used to pass data through the SSO callback
    /// </summary>
    /// <seealso cref="System.IEquatable{QuEve.Infrastructure.Security.SSOState}" />
    public class SSOState : IEquatable<SSOState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SSOState"/> class.
        /// </summary>
        public SSOState()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SSOState"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        public SSOState(string state)
        {
            byte[] bytes = Convert.FromBase64String(state);
            var decodedState = Encoding.UTF8.GetString(bytes);
            var deserialized = JsonConvert.DeserializeObject<SSOState>(decodedState);
            this.StateId = deserialized.StateId;
            this.RedirectUrl = deserialized.RedirectUrl;
            this.AccountId = deserialized.AccountId;
        }

        /// <summary>
        /// Gets or sets the state identifier.
        /// </summary>
        /// <value>
        /// The state identifier.
        /// </value>
        public Guid StateId { get; set; }

        /// <summary>
        /// Gets or sets the redirect URL.
        /// </summary>
        /// <value>
        /// The redirect URL.
        /// </value>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        public int? AccountId { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var result = JsonConvert.SerializeObject(this);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(result));
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(SSOState other)
        {
            if (other == null)
                return false;

            if (StateId != other.StateId || RedirectUrl != other.RedirectUrl || AccountId != other.AccountId)
                return false;

            return true;
        }
    }
}
