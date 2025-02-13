using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _asset;

    public static GameAssets asset
    {
        get
        {
            if (_asset == null) _asset = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _asset;
        }
    }

    public Transform ChatBubble;
}

