using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZarpEspInfo
{
    public const float zarpEspWidth = 22.05714f;
    public const float zarpEspHeight = 125.898f;
    public Vector2 zarpEspPosition;
    public Vector2 zarpEspWarningPosition;
    public GameObject ownObj;
    public GameObject warningLight;
    public float minX;
    public float maxX;
    public ZarpEspInfo(Vector2 _zarpEspPosition, Vector2 _zarpEspWarningPosition, GameObject _ownObj = null, GameObject _warningLight = null)
    {
        zarpEspPosition = _zarpEspPosition;
        zarpEspWarningPosition = _zarpEspWarningPosition;
        ownObj = _ownObj;
        minX = zarpEspPosition.x - zarpEspWidth;
        maxX = zarpEspPosition.x + zarpEspWidth;
        warningLight = _warningLight;

    }
    public void SetWarningPos(Vector2 _warningPos)
    {
        zarpEspWarningPosition = _warningPos;
    }
    public void SetOwnObj(GameObject _ownObj)
    {
        ownObj = _ownObj;
    }
    public void SetWarningLight(GameObject _warningLight)
    {
        this.warningLight = _warningLight;
    }
}
