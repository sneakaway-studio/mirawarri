using UnityEngine;
using System.Collections;

using SA.CrossPlatform.App;

public class UM_BuildInfoExampleOld : MonoBehaviour
{

  
    void Start() {
        UM_iBuildInfo buildInfo = UM_Build.Info;

        Debug.Log("buildInfo.Identifier: " + buildInfo.Identifier);
        Debug.Log("buildInfo.Version: " + buildInfo.Version);
    }


}
