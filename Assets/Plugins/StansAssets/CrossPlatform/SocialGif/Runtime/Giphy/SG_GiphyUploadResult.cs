using System;


[Serializable]
public struct SG_GiphyUploadResult 
{
    private static string k_ShareUrl = "http://giphy.com/gifs/";
    
    [Serializable]
    public struct ResponseData
    {
        public string id;
    }
    
    [Serializable]
    public struct ResponseMeta
    {
        public string msg;
        public int status;
    }

    public ResponseData data;
    public ResponseMeta meta;
    
    
    /// <summary>
    /// Gets a value indicating whether this <see cref="T:SA.Support.Templates.SA_Result"/> is succeeded.
    /// </summary>
    /// <value><c>true</c> if is succeeded; otherwise, <c>false</c>.</value>
    public bool IsSucceeded {
        get {
            return meta.status == 200;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:SA.Support.Templates.SA_Result"/> is failed.
    /// </summary>
    /// <value><c>true</c> if is failed; otherwise, <c>false</c>.</value>
    public bool IsFailed {
        get {
            return !IsSucceeded;
        }
    }

    /// <summary>
    /// Uploaded GIF share url.
    /// </summary>
    public string ShareUrl
    {
        get { return k_ShareUrl + data.id; }
    }
}
