using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hotSpot : MonoBehaviour
{

    public Camera mainCamera;
    [Tooltip("Toggles on and off the camera zoom.")]
    public bool hasZoomBack;
    [Tooltip("Only used if hasZoomBack is true. a positive value will zoom back. A negative value will zoom in.")]
    public float zoomBack;
    //private variables for the zoom effect
    float zoomOriginal;
    float zoomFinal;
    float zoomAct;
    float zoomSmoothSpeed;
    bool zoomingStart, zoomingReturn;
    [Tooltip("Time needed to set the new camera.size. The lower, the faster.")]
    public float zoomSmoothTime;

    [Tooltip("The type of HotSpot.")]
    public hotSpotData.HotSpotMode hsMode;

    [Tooltip("Only used if there the hsMode is set to listCentre. This is the list of positions the camera calculates the centre from.")]
    public List<Transform> targetList;
    [Tooltip("If there is a focusPoint, it will override the Fixed x and y parameters")]
    public Transform focusPoint;
    [Tooltip("Only used if there is no focusPoint. They are used as localPosition")]
    public float FixedX, FixedY, shiftLimitX, shiftLimitY;
    [Tooltip("Toggle the use of a custom smooth time. If set to false, the default smooth time for the normal camera will be used.")]
    public bool useCustomSmoothTime;
    [Tooltip("Time needed to move camera to position. The smaller, the faster.")]
    public float SmoothTime;
    hotSpotData myHSData;

    //crear otros constructores con diferentes argumentos (para zoomv por ej.) si es necesario
    /*public void setHotSpot(hotSpotData _hotSpotData=null)(HotSpotMode _hsMode, List<Transform> _targetList=null, bool _useCustomSmoothTime=false,float _SmoothTime=0.3f,float _FixedX=0,
        float _FixedY=0, bool _hasZoomBack = false, float _zoomBack = 0,float _zoomSmoothTime=0, Camera _mainCamera =null)
    {

        myHSData = _hotSpotData;
    }*/

    void setupHotSpot(hotSpotData _hotSpotData = null)
    {
        if (_hotSpotData == null)
        {
            myHSData = new hotSpotData(hsMode);
            if (hsMode != hotSpotData.HotSpotMode.none)
            {
                if (hsMode == hotSpotData.HotSpotMode.listCentre)
                {
                    myHSData.targetList = targetList;
                }
                else if (hsMode == hotSpotData.HotSpotMode.fixedPos || hsMode == hotSpotData.HotSpotMode.fixedX
                     || hsMode == hotSpotData.HotSpotMode.fixedY)
                {
                    if (focusPoint != null)
                    {
                        myHSData.FixedX = focusPoint.position.x;
                        myHSData.FixedY = focusPoint.position.y;
                    }
                    else
                    {
                        Vector2 aux = new Vector2(FixedX, FixedY);
                        myHSData.FixedX = transform.TransformPoint(aux).x;
                        myHSData.FixedY = transform.TransformPoint(aux).y;
                    }
                }
            }
            if (hasZoomBack)
            {
                myHSData.hasZoomBack = true;
                myHSData.zoomBack = zoomBack;
                if (mainCamera != null)
                {
                    myHSData.mainCamera = mainCamera;
                }
                else
                {
                    myHSData.mainCamera = Camera.main;
                }
                myHSData.zoomSmoothTime = zoomSmoothTime;
            }
            if (useCustomSmoothTime)
            {
                myHSData.useCustomSmoothTime = true;
                myHSData.SmoothTime = SmoothTime;
            }
            
        }
        else
        {
            myHSData = _hotSpotData;
        }
    }

    private void Awake()
    {
        setupHotSpot();
    }

    private void Update()
    {
        if (zoomingStart || zoomingReturn)
        {
            zoomAct = Mathf.SmoothDamp(zoomAct, zoomFinal, ref zoomSmoothSpeed, myHSData.zoomSmoothTime);
            myHSData.mainCamera.orthographicSize = zoomAct;
            if (zoomingReturn)
            {
                if (mainCamera.orthographicSize < zoomFinal + 0.005f && mainCamera.orthographicSize > zoomFinal - 0.005f)
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
            if (myHSData.hasZoomBack)
            {
                zoomingStart = true;
                zoomOriginal = CameraMovement.camSize;
                zoomAct = myHSData.mainCamera.orthographicSize;
                zoomFinal = zoomBack;
            }
            if (hsMode != hotSpotData.HotSpotMode.none)
            {
                CameraMovement.instance.setHotSpot(myHSData);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            Debug.Log("SALE DEL HOTSPOT con mode=" + hsMode);
            if (myHSData.hasZoomBack)
            {
                zoomingStart = false;
                zoomingReturn = true;
                zoomFinal = zoomOriginal;
                zoomAct = mainCamera.orthographicSize;
            }
            CameraMovement.instance.stopHotSpot(myHSData);
        }
    }
}
