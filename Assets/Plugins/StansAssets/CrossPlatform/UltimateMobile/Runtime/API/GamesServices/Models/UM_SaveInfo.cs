using System;
using UnityEngine;

namespace SA.CrossPlatform.GameServices
{
    public class UM_SaveInfo
    {
        private long m_PlayedTimeMillis;
        private long m_ProgressValue;
        
        private string m_Description;
        private Texture2D m_CoverImage = Texture2D.whiteTexture;

        
        /// <summary>
        /// Description to set for the snapshot.
        /// <param name="description">description</param>
        /// </summary>
        public void SetDescription(string description) {
            m_Description = description;
        }

        /// <summary>
        /// The new played time to set for the snapshot.
        /// Value should always be above zero.
        /// <param name="playedTimeMillis">player played time in milliseconds</param>
        /// </summary>
        public void SetPlayedTimeMillis(long playedTimeMillis) {
            m_PlayedTimeMillis = playedTimeMillis;
        }

        /// <summary>
        /// The new progress value to set for the snapshot.
        /// Value should always be above zero.
        /// <param name="progressValue">player progress value</param>
        /// </summary>
        public void SetProgressValue(long progressValue) {
            m_ProgressValue = progressValue;
        }
        
        /// <summary>
        /// Cover image to set for the snapshot.
        /// <param name="coverImage"></param>
        /// </summary>
        public void SetCoverImage(Texture2D coverImage) {
            m_CoverImage = coverImage;
        }

        
        
        /// <summary>
        /// Retrieves the played time of this snapshot in milliseconds. 
        /// </summary>
        public long PlayedTime {
            get {
                return m_PlayedTimeMillis;
            }
        }

        /// <summary>
        /// Retrieves the progress value for this snapshot. 
        /// </summary>
        public long ProgressValue {
            get {
                return m_ProgressValue;
            }
        }
        
        /// <summary>
        /// Retrieves the description of this snapshot.
        /// </summary>
        public string Description {
            get {
                return m_Description;
            }
        }


        public virtual void LoadCoverImage(Action<Texture2D> callback)
        {
            callback.Invoke(m_CoverImage);
        }
    }
}