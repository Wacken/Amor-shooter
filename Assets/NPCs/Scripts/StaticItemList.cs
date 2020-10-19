using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticItemList : MonoBehaviour {

    public static List<GameObject> HeadItems, FaceItems, BodyItems; 
    public static List<Sprite> SymbolsHead, SymbolsFace, SymbolsBody;
    public List<GameObject> headItems, faceItems, bodyItems;
    public List<Sprite> symbolsHead, symbolsFace, symbolsBody;

    // Use this for initialization
    void Awake () {
        HeadItems = headItems;
        FaceItems = faceItems;
        BodyItems = bodyItems;
        SymbolsHead = symbolsHead;
        SymbolsFace = symbolsFace;
        SymbolsBody = symbolsBody;
    }
}
