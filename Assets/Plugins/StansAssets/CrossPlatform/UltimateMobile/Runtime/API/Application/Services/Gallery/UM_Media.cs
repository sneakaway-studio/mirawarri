using System;
using UnityEngine;


namespace SA.CrossPlatform.App
{
    /// <summary>
    /// Picked image from gallery representation
    /// </summary>
    [Serializable]
    public class UM_Media
    {
        [SerializeField] string m_Path;
        [SerializeField] UM_MediaType m_Type;
        [SerializeField] private byte[] m_RawBytes;
        [SerializeField] Texture2D m_Thumbnail;

        public UM_Media(Texture2D thumbnail, byte[] rawBytes, string path, UM_MediaType type) 
        {
            m_Path = path;
            m_Type = type;
            m_Thumbnail = thumbnail;
            m_RawBytes = rawBytes;
        }

        /// <summary>
        /// Device local path to the current media file.
        /// </summary>
        /// <value>The path.</value>
        public string Path 
        {
            get { return m_Path; }
        }

        /// <summary>
        /// Media file thumbnail.
        /// </summary>
        public Texture2D Thumbnail 
        {
            get { return m_Thumbnail; }
        }

        /// <summary>
        /// Type of yhe media
        /// </summary>
        public UM_MediaType Type 
        {
            get { return m_Type; }
        }

        /// <summary>
        /// Media raw bytes
        /// </summary>
        public byte[] RawBytes
        {
            get { return m_RawBytes; }
        }
        
    }
}