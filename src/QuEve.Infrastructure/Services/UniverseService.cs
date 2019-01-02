using EVEStandard;
using EVEStandard.Models.API;
using Microsoft.EntityFrameworkCore;
using QuEve.Core.Entities;
using QuEve.Core.Interfaces;
using QuEve.Infrastructure.Data;
using System;
using System.Threading.Tasks;
using ESIModels = EVEStandard.Models;

namespace QuEve.Infrastructure.Services
{
    /// <summary>
    /// Universe Service
    /// </summary>
    /// <seealso cref="QuEve.Core.Interfaces.IUniverseService" />
    public class UniverseService : IUniverseService
    {
        private readonly EFDbContext _context;
        private readonly EVEStandardAPI _esiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniverseService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="esiClient">The esi client.</param>
        public UniverseService(EFDbContext context, EVEStandardAPI esiClient)
        {
            _context = context;
            _esiClient = esiClient;
        }

        /// <summary>
        /// Gets the alliance.
        /// </summary>
        /// <param name="allianceId">The alliance identifier.</param>
        /// <returns></returns>
        public async Task<Alliance> GetAlliance(int? allianceId)
        {
            // Alliance membership is optional
            if (allianceId == null)
                return null;

            // Check if local copy exists and if the ESI response is still cached.
            var alliance = await _context.Alliances.FirstOrDefaultAsync(x => x.AllianceId == allianceId);
            if (alliance == null || alliance.CachedUntil < DateTime.UtcNow)
            {
                var allianceInfo = await _esiClient.Alliance.GetAllianceInfoV3Async(allianceId.Value, alliance?.ETag);
                if (allianceInfo.NotModified)
                    return alliance;

                if (allianceInfo.Model == null)
                    return null;

                if (alliance == null)
                {
                    alliance = await CreateAlliance(allianceId.Value, allianceInfo);
                }
                else
                {
                    await UpdateAlliance(alliance, allianceInfo);
                }
            }

            return alliance;
        }

        /// <summary>
        /// Creates the alliance.
        /// </summary>
        /// <param name="allianceId">The alliance identifier.</param>
        /// <param name="allianceInfo">The alliance information.</param>
        /// <returns></returns>
        private async Task<Alliance> CreateAlliance(int allianceId, ESIModelDTO<ESIModels.Alliance> allianceInfo)
        {
            var alliance = new Alliance(allianceId);

            UpdateAllianceProperties(alliance, allianceInfo);

            _context.Alliances.Add(alliance);
            await _context.SaveChangesAsync();

            return alliance;
        }

        /// <summary>
        /// Updates the alliance.
        /// </summary>
        /// <param name="alliance">The alliance.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">alliance</exception>
        private async Task UpdateAlliance(Alliance alliance)
        {
            if (alliance == null)
                throw new ArgumentNullException(nameof(alliance));

            _context.Alliances.Attach(alliance);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the alliance.
        /// </summary>
        /// <param name="alliance">The alliance.</param>
        /// <param name="allianceInfo">The alliance information.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">alliance</exception>
        private async Task UpdateAlliance(Alliance alliance, ESIModelDTO<ESIModels.Alliance> allianceInfo)
        {
            if (alliance == null)
                throw new ArgumentNullException(nameof(alliance));

            UpdateAllianceProperties(alliance, allianceInfo);
            await UpdateAlliance(alliance);
        }

        /// <summary>
        /// Updates the alliance properties.
        /// </summary>
        /// <param name="alliance">The alliance.</param>
        /// <param name="allianceInfo">The alliance information.</param>
        private void UpdateAllianceProperties(Alliance alliance, ESIModelDTO<ESIModels.Alliance> allianceInfo)
        {
            alliance.CachedUntil = allianceInfo.Expires?.UtcDateTime ?? DateTime.UtcNow.AddHours(24);
            alliance.ETag = allianceInfo.ETag;
            alliance.LastUpdated = DateTime.UtcNow;
            alliance.Name = allianceInfo.Model.Name;
            alliance.Ticker = allianceInfo.Model.Ticker;
        }

        /// <summary>
        /// Gets the character.
        /// </summary>
        /// <param name="characterId">The character identifier.</param>
        /// <returns></returns>
        public async Task<Character> GetCharacter(int characterId)
        {
            var character = await _context.Characters
                .Include(x => x.Corporation)
                .Include(x => x.Corporation.Alliance)
                .FirstOrDefaultAsync(x => x.CharacterId == characterId);

            if (character == null || character.CachedUntil < DateTime.UtcNow)
            {
                var characterInfo = await _esiClient.Character.GetCharacterPublicInfoV4Async(characterId, character?.ETag);
                if (characterInfo.NotModified)
                    return character;

                if (characterInfo.Model == null)
                    return null;

                if (character == null)
                {
                    character = await CreateCharacter(characterId, characterInfo);
                }
                else
                {
                    await UpdateCharacter(character, characterInfo);
                }
            }

            return character;
        }

        /// <summary>
        /// Creates the character.
        /// </summary>
        /// <param name="characterId">The character identifier.</param>
        /// <param name="characterInfo">The character information.</param>
        /// <returns></returns>
        private async Task<Character> CreateCharacter(int characterId, ESIModelDTO<ESIModels.CharacterInfo> characterInfo)
        {
            var account = new Account();
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var character = new Character(characterId, account.AccountId);

            await UpdateCharacterProperties(character, characterInfo);

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            return character;
        }

        /// <summary>
        /// Updates the character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">character</exception>
        public async Task UpdateCharacter(Character character)
        {
            if (character == null)
                throw new ArgumentNullException(nameof(character));

            _context.Characters.Attach(character);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="characterInfo">The character information.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">character</exception>
        private async Task UpdateCharacter(Character character, ESIModelDTO<ESIModels.CharacterInfo> characterInfo)
        {
            if (character == null)
                throw new ArgumentNullException(nameof(character));

            await UpdateCharacterProperties(character, characterInfo);
            await UpdateCharacter(character);
        }

        /// <summary>
        /// Updates the character properties.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="characterInfo">The character information.</param>
        /// <returns></returns>
        private async Task UpdateCharacterProperties(Character character, ESIModelDTO<ESIModels.CharacterInfo> characterInfo)
        {
            character.CachedUntil = characterInfo.Expires?.UtcDateTime ?? DateTime.UtcNow.AddHours(24);
            character.Corporation = await GetCorporation(characterInfo.Model.CorporationId);
            character.ETag = characterInfo.ETag;
            character.LastUpdated = DateTime.UtcNow;
            character.Name = characterInfo.Model.Name;
        }

        /// <summary>
        /// Gets the corporation.
        /// </summary>
        /// <param name="corporationId">The corporation identifier.</param>
        /// <returns></returns>
        public async Task<Corporation> GetCorporation(int corporationId)
        {
            var corporation = await _context.Corporations
                .Include(x => x.Alliance)
                .FirstOrDefaultAsync(x => x.CorporationId == corporationId);

            if (corporation == null || corporation.CachedUntil < DateTime.UtcNow)
            {
                var corporationInfo = await _esiClient.Corporation.GetCorporationInfoV4Async(corporationId, corporation?.ETag);
                if (corporationInfo.NotModified)
                    return corporation;

                if (corporationInfo.Model == null)
                    return null;

                if (corporation == null)
                {
                    corporation = await CreateCorporation(corporationId, corporationInfo);
                }
                else
                {
                    await UpdateCorporation(corporation, corporationInfo);
                }
            }

            return corporation;
        }

        /// <summary>
        /// Creates the corporation.
        /// </summary>
        /// <param name="corporationId">The corporation identifier.</param>
        /// <param name="corporationInfo">The corporation information.</param>
        /// <returns></returns>
        private async Task<Corporation> CreateCorporation(int corporationId, ESIModelDTO<ESIModels.CorporationInfo> corporationInfo)
        {
            var corporation = new Corporation(corporationId);

            await UpdateCorporationProperties(corporation, corporationInfo);

            _context.Corporations.Add(corporation);
            await _context.SaveChangesAsync();

            return corporation;
        }

        /// <summary>
        /// Updates the corporation.
        /// </summary>
        /// <param name="corporation">The corporation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">corporation</exception>
        public async Task UpdateCorporation(Corporation corporation)
        {
            if (corporation == null)
                throw new ArgumentNullException(nameof(corporation));

            _context.Corporations.Attach(corporation);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the corporation.
        /// </summary>
        /// <param name="corporation">The corporation.</param>
        /// <param name="corporationInfo">The corporation information.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">corporation</exception>
        private async Task UpdateCorporation(Corporation corporation, ESIModelDTO<ESIModels.CorporationInfo> corporationInfo)
        {
            if (corporation == null)
                throw new ArgumentNullException(nameof(corporation));

            await UpdateCorporationProperties(corporation, corporationInfo);
            await UpdateCorporation(corporation);
        }

        /// <summary>
        /// Updates the corporation properties.
        /// </summary>
        /// <param name="corporation">The corporation.</param>
        /// <param name="corporationInfo">The corporation information.</param>
        /// <returns></returns>
        private async Task UpdateCorporationProperties(Corporation corporation, ESIModelDTO<ESIModels.CorporationInfo> corporationInfo)
        {
            corporation.Alliance = await GetAlliance(corporationInfo.Model.AllianceId);
            corporation.CachedUntil = corporationInfo.Expires?.UtcDateTime ?? DateTime.UtcNow.AddHours(24);
            corporation.ETag = corporationInfo.ETag;
            corporation.LastUpdated = DateTime.UtcNow;
            corporation.Name = corporationInfo.Model.Name;
            corporation.Ticker = corporationInfo.Model.Ticker;
        }
    }
}
