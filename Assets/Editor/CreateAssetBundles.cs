using UnityEditor;
using System.IO;

public class CreateAssetBundles {

	[MenuItem("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles(){
		string assetBundleDirectory = "Assets/AssetBundles";
		if(!Directory.Exists(assetBundleDirectory)){
			Directory.CreateDirectory(assetBundleDirectory);
		}
		// https://docs.unity3d.com/ScriptReference/BuildTarget.html
		BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSXIntel);
	}
}
