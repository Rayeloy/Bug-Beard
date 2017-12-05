using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hotSpot : MonoBehaviour {

    public Camera mainCamera;
    [Tooltip("Toggles on and off the camera zoom.")]
    public bool hasZoomBack;
    [Tooltip("Only used if hasZoomBack is true. a positive value will zoom back. A negative value will zoom in.")]
    public float zoomBack;
    float zoomOriginal;
    float zoomFinal;
    float zoomAct;
    [Tooltip("Time needed to set the new camera.size. The lower, the faster.")]
    public float zoomSmoothTime;
    float zoomSmoothSpeed;
    bool zoomingStart, zoomingReturn;

    [Tooltip("The type of HotSpot.")]
    public HotSpotMode hsMode;
    public enum HotSpotMode
    {
        none,
        fixedX,
        fixedY,
        fixedPos,
        listCentre
    }

    [Tooltip("Only used if there the hsMode is set to listCentre. This is the list of positions the camera calculates the centre from.")]
    public List<Transform> targetList;
    [Tooltip("If there is a focusPoint, it will override the Fixed x and y parameters")]
    public Transform focusPoint;
    private Vector3 focusPointPos;
    [Tooltip("Only used if there is no focusPoint. They are used as localPosition")]
    public float FixedX, FixedY, shiftLimitX, shiftLimitY;
    [Tooltip("Toggle the use of a custom smooth time. If set to false, the default smooth time for the normal camera will be used.")]
    public bool useCustomSmoothTime;
    [Tooltip("Time needed to move camera to position. The smaller, the faster.")]
    public float SmoothTime;

    //crear otros constructores con diferentes argumentos (para zoomv por ej.) si es necesario
    public hotSpot(HotSpotMode _hsMode, List<Transform> _targetList=null, bool _useCustomSmoothTime=false,float _SmoothTime=0.3f,float _FixedX=0,
        float _FixedY=0, bool _hasZoomBack = false, float _zoomBack = 0,float _zoomSmoothTime=0, Camera _mainCamera =null)
    {
        hsMode = _hsMode;
        targetList = _targetList;
        SmoothTime = _SmoothTime;
        FixedX = _FixedX;
        FixedY = _FixedY;
        hasZoomBack = _hasZoomBack;
        zoomBack = _zoomBack;
        zoomSmoothTime = _zoomSmoothTime;
        mainCamera = _mainCamera;

    }

    private void Awake()
    {
        if(focusPoint!=null)
        focusPointPos = focusPoint.position;
        if (focusPoint != null)
        {
            FixedX = focusPointPos.x;
            FixedY = focusPointPos.y;
        }
        else
        {
            Vector2 aux =transform.TransformPoint(new Vector2(FixedX, FixedY));
            FixedX = aux.x;
            FixedY = aux.y;
        }
    }

    private void Update()
    {
        if (zoomingStart||zoomingReturn)
        {
            zoomAct = Mathf.SmoothDamp(zoomAct, zoomFinal, ref zoomSmoothSpeed, zoomSmoothTime);
            mainCamera.orthographicSize = zoomAct;
            if (zoomingReturn)
            {
                if(mainCamera.orthographicSize<zoomFinal+0.005f && mainCamera.orthographicSize > zoomFinal - 0.005f)
                {
                    mainCamera.orthographicSize = zoomFinal;
                    zoomingReturn = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (hasZoomBack)
            {
                zoomingStart = true;
                zoomOriginal = zoomAct = mainCamera.orthographicSize;
                zoomFinal = zoomBack;
            }
            if (hsMode != HotSpotMode.none)
            {
                CameraMovement.instance.setHotSpot(this);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (hasZoomBack)
            {
                zoomingStart = false;
                zoomingReturn = true;
                zoomFinal = zoomOriginal;
                zoomAct = mainCamera.orthographicSize;
            }
                CameraMovement.instance.stopHotSpot();
        }
    }
}
