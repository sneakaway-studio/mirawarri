using System;
using System.Collections.Generic;
using System.Linq;
using SA.Foundation.Templates;
using UnityEngine;

using SA.Foundation.Async;

namespace SA.CrossPlatform.GameServices
{
    /// <summary>
    /// A client to interact with saved games functionality.
    /// </summary>
    internal class UM_EditorSavedGamesClient : UM_iSavedGamesClient
    {

        private const string PP_EDITOR_SAVES_KEY = "UM_SAVED_GAMES_DATA";


       

        public void FetchSavedGames(Action<UM_SavedGamesMetadataResult> callback) {

            UM_SavedGamesMetadataResult loadResult = new UM_SavedGamesMetadataResult();

            EditorSavedGamesList editorGamesList = LoadSavesList();
            foreach (EditorSavedGame game in editorGamesList.Saves) {
                loadResult.AddMetadata(game);
            }

            SA_Coroutine.WaitForSeconds(1.5f, () => {
                callback.Invoke(loadResult);
            });

        }
        
        public void SaveGame(string name, byte[] data, Action<SA_Result> callback) {
            var editorGamesList = LoadSavesList();
            var game = editorGamesList.GetByName(name);
            if(game == null) {
                game = new EditorSavedGame();
                game.Name = name;
                editorGamesList.Saves.Add(game);
            }

            game.GameData = System.Text.Encoding.UTF8.GetString(data);
            EditorSaveGames(editorGamesList);
           

            SA_Coroutine.WaitForSeconds(1.5f, () => {
                callback.Invoke(new SA_Result());
            });
        }

        public void SaveGameWithMeta(string name, byte[] data, UM_SaveInfo meta, Action<SA_Result> callback)
        {
            var extendedData = Combine(BitConverter.GetBytes(meta.ProgressValue),
                BitConverter.GetBytes(meta.PlayedTime),
                data);

            SaveGame(name, extendedData, callback);
        }
        
        private byte[] Combine(params byte[][] arrays)
        {
            var rv = new byte[arrays.Sum(a => a.Length)];
            var offset = 0;
            foreach (var array in arrays) {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
        
        public void LoadGameData(UM_iSavedGameMetadata game, Action<UM_SavedGameDataLoadResult> callback)
        {
            LoadFromPlayerPrefs(game, false, callback);
        }

        public void LoadGameWithMeta(UM_iSavedGameMetadata game, Action<UM_SavedGameDataLoadResult> callback)
        {
            LoadFromPlayerPrefs(game, true, callback);
        }
        
        public void LoadFromPlayerPrefs(UM_iSavedGameMetadata game, bool parseMeta, Action<UM_SavedGameDataLoadResult> callback) {
           
            EditorSavedGamesList editorGamesList = LoadSavesList();
            var editorGame = editorGamesList.GetByName(game.Name);

            UM_SavedGameDataLoadResult loadResult;
            if(editorGame != null)
            {
                var BytesArrayData = editorGame.GameData.ToBytes();
                if (parseMeta)
                {
                    var meta = new UM_SaveInfo();
                    meta.SetProgressValue(BitConverter.ToInt64(BytesArrayData, 0)); 
                    meta.SetPlayedTimeMillis(BitConverter.ToInt64(BytesArrayData, 8));

                    var userData = new byte[BytesArrayData.Length - 16];
                    Array.Copy(BytesArrayData, 16, userData, 0, userData.Length);
                    loadResult = new UM_SavedGameDataLoadResult(userData, meta);
                }
                else
                {
                    loadResult = new UM_SavedGameDataLoadResult(BytesArrayData, new UM_SaveInfo());
                }
            } else {
                SA_Error error = new SA_Error(100, "Saved game with name: " + game.Name + " wasn't found");
                loadResult = new UM_SavedGameDataLoadResult(error);
            }

            EditorSaveGames(editorGamesList);

            SA_Coroutine.WaitForSeconds(1.5f, () => {
                callback.Invoke(loadResult);
            });
        }



        public void Delete(UM_iSavedGameMetadata game, Action<SA_Result> callback) {
            EditorSavedGamesList editorGamesList = LoadSavesList();
            var editorGame = editorGamesList.GetByName(game.Name);

            if(editorGame != null) {
                editorGamesList.Saves.Remove(editorGame);
                EditorSaveGames(editorGamesList);
            }

            SA_Coroutine.WaitForSeconds(1.5f, () => {
                callback.Invoke(new SA_Result());
            });
        }



        private EditorSavedGamesList LoadSavesList() {
            if(PlayerPrefs.HasKey(PP_EDITOR_SAVES_KEY)) {
                string json = PlayerPrefs.GetString(PP_EDITOR_SAVES_KEY);
                return JsonUtility.FromJson<EditorSavedGamesList>(json);
            } else {
                return new EditorSavedGamesList();
            }
        }


        private void EditorSaveGames(EditorSavedGamesList list) {
            string json = JsonUtility.ToJson(list);
            PlayerPrefs.SetString(PP_EDITOR_SAVES_KEY, json);
        }


        [Serializable]
        private class EditorSavedGame : UM_iSavedGameMetadata {

            [SerializeField] string m_name = null;
            [SerializeField] public string GameData = null;

            public string DeviceName  {
                get {
                    return "Editor";
                }
            }

            public string Name {
                get {
                    return m_name;
                }

                set {
                    m_name = value;
                }
            }
        }


        [Serializable]
        private class EditorSavedGamesList
        {
            [SerializeField] List<EditorSavedGame> m_saves = new List<EditorSavedGame>();
            public List<EditorSavedGame> Saves {
                get {
                    return m_saves;
                }
            }


            public EditorSavedGame GetByName(string name) {
                foreach(var game in m_saves) {
                    if(game.Name.Equals(name)) {
                        return game;
                    }
                }

                return null;
            }
        }

    }
}