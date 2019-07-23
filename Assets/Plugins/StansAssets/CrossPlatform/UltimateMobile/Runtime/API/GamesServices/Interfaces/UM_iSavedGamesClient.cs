using System;
using SA.Foundation.Templates;

namespace SA.CrossPlatform.GameServices
{
    /// <summary>
    /// A client to interact with saved games functionality.
    /// </summary>
    public interface UM_iSavedGamesClient 
    {

        /// <summary>
        /// Retrieves all available saved games.
        /// </summary>
        void FetchSavedGames(Action<UM_SavedGamesMetadataResult> callback);


        /// <summary>
        /// Saves game data under the specified name.
        /// If save with such name already exists, save game data will be replaced.
        /// </summary>
        void SaveGame(string name, byte[] data, Action<SA_Result> callback);
        
        /// <summary>
        /// Saves game data under the specified name.
        /// If save with such name already exists, save game data will be replaced.
        /// </summary>
        void SaveGameWithMeta(string name, byte[] data, UM_SaveInfo meta, Action<SA_Result> callback);

        /// <summary>
        /// Loads specific saved game data.
        /// </summary>
        void LoadGameData(UM_iSavedGameMetadata game, Action<UM_SavedGameDataLoadResult> callback);

        /// <summary>
        /// Loads specific saved game data with attempt to get additional meta data out of it.
        /// </summary>
        /// <param name="game">game to load.</param>
        /// <param name="callback">action result callback.</param>
        void LoadGameWithMeta(UM_iSavedGameMetadata game, Action<UM_SavedGameDataLoadResult> callback);

        /// <summary>
        /// Deletes a specific saved game.
        /// </summary>
        void Delete(UM_iSavedGameMetadata game, Action<SA_Result> callback);
    }
}