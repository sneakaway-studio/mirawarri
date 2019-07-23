using UnityEngine;
using System;
using System.Collections;

using SA.Foundation.Templates;


namespace SA.CrossPlatform.GameServices
{

    /// <summary>
    /// Game load result object.
    /// </summary>
    [Serializable]
    public class UM_SavedGameDataLoadResult : SA_Result
    {
        private byte[] m_Data;
        private UM_SaveInfo m_Meta;
        

        public UM_SavedGameDataLoadResult(byte[] data, UM_SaveInfo meta) {
            m_Data = data;
            m_Meta = meta;
        }

        public UM_SavedGameDataLoadResult(SA_Error error): base(error) { }




        /// <summary>
        /// Loaded game data
        /// </summary>
        public byte[] Data {
            get {
                return m_Data;
            }
        }
        
        /// <summary>
        /// Loaded game meta
        /// </summary>
        public UM_SaveInfo Meta {
            get {
                return m_Meta;
            }
        }


    }
}