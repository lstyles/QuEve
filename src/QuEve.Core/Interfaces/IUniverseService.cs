using QuEve.Core.Entities;
using System.Threading.Tasks;

namespace QuEve.Core.Interfaces
{
    /// <summary>
    /// Universe Service interface
    /// </summary>
    public interface IUniverseService
    {
        /// <summary>
        /// Gets the alliance.
        /// </summary>
        /// <param name="allianceId">The alliance identifier.</param>
        /// <returns></returns>
        Task<Alliance> GetAlliance(int? allianceId);

        /// <summary>
        /// Gets the character.
        /// </summary>
        /// <param name="characterId">The character identifier.</param>
        /// <returns></returns>
        Task<Character> GetCharacter(int characterId);

        /// <summary>
        /// Updates the character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        Task UpdateCharacter(Character character);

        /// <summary>
        /// Gets the corporation.
        /// </summary>
        /// <param name="corporationId">The corporation identifier.</param>
        /// <returns></returns>
        Task<Corporation> GetCorporation(int corporationId);
    }
}
