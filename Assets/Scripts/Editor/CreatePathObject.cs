using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreatePathObject : MonoBehaviour {

    [MenuItem("Assets/Create/Scriptable/PathObject")]
    public static void Create()
    {
        PathObject asset = ScriptableObject.CreateInstance<PathObject>();
        AssetDatabase.CreateAsset(asset, "Assets/Paths/NewPathObject.asset");
        AssetDatabase.SaveAssets();
    }
}
