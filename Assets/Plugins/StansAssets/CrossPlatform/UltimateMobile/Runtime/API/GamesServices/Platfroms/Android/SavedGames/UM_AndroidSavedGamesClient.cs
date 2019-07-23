using System;
using SA.Android.GMS.Games;
using SA.Foundation.Templates;


namespace SA.CrossPlatform.GameServices
{
    /// <summary>
    /// A client to interact with saved games functionality.
    /// </summary>
    internal class UM_AndroidSavedGamesClient : UM_AbstractSavedGamesClient, UM_iSavedGamesClient
    {
        public void FetchSavedGames(Action<UM_SavedGamesMetadataResult> callback) {
            var client = AN_Games.GetSnapshotsClient();
            client.Load((result) => {

            UM_SavedGamesMetadataResult loadResult;

            if (result.IsSucceeded) {
                    loadResult = new UM_SavedGamesMetadataResult();
                    foreach (var meta in result.Snapshots) {
                        var an_meta = new UM_AndroidSavedGameMetadata(meta);
                        loadResult.AddMetadata(an_meta);
                    }
                } else {
                    loadResult = new UM_SavedGamesMetadataResult(result.Error);
                }


                callback.Invoke(loadResult);
            });
        }

        public void Delete(UM_iSavedGameMetadata game, Action<SA_Result> callback) {
            UM_AndroidSavedGameMetadata an_meta = (UM_AndroidSavedGameMetadata)game;

            var client = AN_Games.GetSnapshotsClient();
            client.Delete(an_meta.NativeMeta, (result) => {
                callback.Invoke(result);
            });
        }

        public void SaveGame(string name, byte[] data, Action<SA_Result> callback)
        {
            SaveAndroidSnapShotGame(name, data, AN_SnapshotMetadataChange.EMPTY_CHANGE, callback);
        }

        public void SaveGameWithMeta(string name, byte[] data, UM_SaveInfo meta, Action<SA_Result> callback)
        {
            var androidMeta = new AN_SnapshotMetadataChange.Builder();
            androidMeta.SetProgressValue(meta.ProgressValue);
            androidMeta.SetPlayedTimeMillis(meta.PlayedTime);
            androidMeta.SetDescription(meta.Description);
           
            //we know this is going to by synchronous
            meta.LoadCoverImage((texture) =>
            {
                androidMeta.SetCoverImage(texture);
            });
            
            SaveAndroidSnapShotGame(name, data, androidMeta.Build(), callback);
        }

        private void SaveAndroidSnapShotGame(string name, byte[] data, AN_SnapshotMetadataChange meta,  Action<SA_Result> callback)
        {
            var client = AN_Games.GetSnapshotsClient();
            //This resolution is picked since this is the only we can currently implement for iOS
            //so we pick same one for android, just to be consistent.
            var conflictPolicy = AN_SnapshotsClient.ResolutionPolicy.MOST_RECENTLY_MODIFIED;

            client.Open(name, true, conflictPolicy, (result) => {
                if (result.IsSucceeded) {
                    var snapshot = result.Data;

                    snapshot.WriteBytes(data);
                    client.CommitAndClose(snapshot, meta, (commitResult) => {
                        ReportGameSave(name, result);
                        callback.Invoke(result);
                    });

                } else {
                    callback.Invoke(result);
                }
            });
        }

        public void LoadGameData(UM_iSavedGameMetadata game, Action<UM_SavedGameDataLoadResult> callback)
        {
            LoadGameWithMeta(game, callback);
        }

        public void LoadGameWithMeta(UM_iSavedGameMetadata game, Action<UM_SavedGameDataLoadResult> callback)
        {
            var client = AN_Games.GetSnapshotsClient();
            var conflictPolicy = AN_SnapshotsClient.ResolutionPolicy.LAST_KNOWN_GOOD;

            client.Open(game.Name, true, conflictPolicy, (result) => {

                UM_SavedGameDataLoadResult loadResult;
                if (result.IsSucceeded) {
                    var snapshot = result.Data;
                    var data = snapshot.ReadFully();

                    client.CommitAndClose(snapshot, AN_SnapshotMetadataChange.EMPTY_CHANGE, (commitResult) => {
                        if (commitResult.IsSucceeded)
                        {
                            loadResult = new UM_SavedGameDataLoadResult(data, new AndroidSaveInfo(commitResult.Metadata));
                        } else {
                            loadResult = new UM_SavedGameDataLoadResult(commitResult.Error);
                        }

                        callback.Invoke(loadResult);
                    });
                } else {
                    loadResult = new UM_SavedGameDataLoadResult(result.Error);
                    callback.Invoke(loadResult);
                }
            });
        }
        
    }
}