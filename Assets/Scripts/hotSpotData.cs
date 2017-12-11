using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hotSpotData
{
    public HotSpotMode hsMode;
    public List<Transform> targetList;
    public bool useCustomSmoothTime, hasZoomBack;
    public float SmoothTime, zoomBack, zoomSmoothTime;
    public float FixedX, FixedY;
    public Camera mainCamera;

    public enum HotSpotMode
    {
        none,
        fixedX,
        fixedY,
        fixedPos,
        listCentre
    }

    public hotSpotData(HotSpotMode _hsMode, List<Transform> _targetList = null, bool _useCustomSmoothTime = false, float _SmoothTime = 0.3f, float _FixedX = 0,
        float _FixedY = 0, bool _hasZoomBack = false, float _zoomBack = 0, float _zoomSmoothTime = 0, Camera _mainCamera = null)
    {
        hsMode = _hsMode;
        targetList = _targetList;
        useCustomSmoothTime = _useCustomSmoothTime;
        SmoothTime = _SmoothTime;
        FixedX = _FixedX;
        FixedY = _FixedY;
        hasZoomBack = _hasZoomBack;
        zoomBack = _zoomBack;
        zoomSmoothTime = _zoomSmoothTime;
        mainCamera = _mainCamera;
    }
    public hotSpotData(bool _hasZoomBack = false, float _zoomBack = 0, Camera _mainCamera = null, bool _useCustomSmoothTime = false, float _zoomSmoothTime = 0)
    {
        hsMode = HotSpotMode.none;
        hasZoomBack = _hasZoomBack;
        zoomBack = _zoomBack;
        useCustomSmoothTime = _useCustomSmoothTime;
        if (useCustomSmoothTime)
        {
            zoomSmoothTime = _zoomSmoothTime;
        }
        if (_mainCamera != null)
        {
            mainCamera = _mainCamera;
        }
        else
        {
            mainCamera = Camera.main;
        }
    }
}
