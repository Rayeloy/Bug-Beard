﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public static CameraMovement instance;

    public bool SpringCameraNewMode;

    public EasingCurves EC;
    private Transform camTrans;
    public Transform PlayerT;//Camera Target en el player
    public Transform Player;
    private Rigidbody2D playerTRB;//Rigid Body del Camera Target en el Player
    public Vector3 cameraPosOr;
    private Vector3 cameraPosAct;
    private Vector3 cameraPosFinal;
    private Vector3 lastCameraPosFinal;

    public float fowDist;
    public float runFowDist;
    //SpringCamMode1
    [Header("Old Spring Camera Mode 1")]
    public float maxTime;
    private float actTime;
    private float progress;

    [Header("Old Spring Camera Mode 2")]
    //SpringCamMode2
    public float camAcc;
    private float camDeacc;
    public float camMaxSpeed;
    private float camBreakDist;//distancia a la que empieza a frenar
    public float camTimeToBreak;
    private float playerTSpeed;
    private float newPlayerTSpeed;
    private float playerTLastPosX;

    private PlayerMovement.pmoveState lastPMState;
    private float speedTimer;
    public float speedFreq;

    Collider2D cameraLimits;

    //------------------------------CAMERA FINAL VERSION----------------------------
    private cameraMode camMode;
    public Vector2 focusPosition;
    hotSpot currentHotSpot;
    private List<Transform> hsTargets;

    float smoothVelocityX, smoothVelocityY;
    public float verticalSmoothTime, horizontalSmoothTime;
    float finalVSmoothT,finalHSmoothT;
    [HideInInspector]
    public bool LookDown = false, LookUp=false;
    [Tooltip("Distancia que se mueve la camara al mirar hacia abajo o arriba pulsando s/down o w/up.")]
    public float LookDownDist, LookUpDist;

    private void Awake()
    {
        camTrans = transform;
        progress = 1.5f;
        playerTRB = PlayerT.GetComponent<Rigidbody2D>();
        actTime = maxTime + 1;//empezar apagado
        instance = this;
        camMode = cameraMode.focusPlayerBox;
        playerTSpeed = 0;
        newPlayerTSpeed = 0;
        speedTimer = 0;
        cameraLimits = GetComponent<Collider2D>();
        playerCol = Player.GetComponent<Collider2D>();
    }

    public enum cameraMode
    {
        focusPlayer,
        focusPlayerBox,
        focusHotSpot,
        focusList,
    }

    void Start()
    {
        if (PlayerMovement.instance.pmState == PlayerMovement.pmoveState.stopRight)
        {
            PlayerT.localPosition = new Vector3(cameraPosOr.x + fowDist, cameraPosOr.y, cameraPosOr.z);
        }
        else if (PlayerMovement.instance.pmState == PlayerMovement.pmoveState.stopLeft)
        {
            PlayerT.localPosition = new Vector3(cameraPosOr.x - fowDist, cameraPosOr.y, cameraPosOr.z);
        }
        cameraPosFinal = PlayerT.localPosition;
        lastPMState = PlayerMovement.instance.pmState;
    }
    void LateUpdate()
    {
        lastCameraPosFinal = cameraPosFinal;
        finalVSmoothT = verticalSmoothTime;
        finalHSmoothT = horizontalSmoothTime;
        switch (camMode)
        {
            case cameraMode.focusPlayer:
                focusPlayer();
                break;
            case cameraMode.focusPlayerBox:
                PlayerCameraBox.instance.KonoUpdate();
                focusPlayerBox();
                break;
            case cameraMode.focusHotSpot:
                PlayerCameraBox.instance.KonoUpdate();
                focusPlayerBox();
                focusHotSpot();
                break;
            case cameraMode.focusList:
                break;
        }
        if (LookUp)
        {
            focusPosition.y += LookUpDist;
        }
        else if(LookDown)
        {
            focusPosition.y -= LookDownDist;
        }
        //Debug.Log("PREFINAL CAMERA POS= " + transform.position);
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, finalVSmoothT);
        focusPosition.x = Mathf.SmoothDamp(transform.position.x, focusPosition.x, ref smoothVelocityX, finalHSmoothT);
        //focusPosition.y = Mathf.Lerp(transform.position.y, focusPosition.y, Time.deltaTime*4);
        //focusPosition.x = Mathf.Lerp(transform.position.x, focusPosition.x, Time.deltaTime*4);
        //Debug.Log("FINAL CAMERA POS= " + focusPosition);
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    void focusPlayer()//sigue al jugador con camara muelle a la misma altura siempre
    {
        bool recentChange = false;
        //Debug.Log("playerMovementState=" + PlayerMovement.instance.pmState.ToString());
        if (lastPMState != PlayerMovement.instance.pmState)
        {//cambiar camPosFinal?
            switch (PlayerMovement.instance.pmState)
            {
                case PlayerMovement.pmoveState.stopRight:
                    cameraPosFinal = new Vector3(cameraPosOr.x + fowDist, cameraPosOr.y, cameraPosOr.z);
                    break;
                case PlayerMovement.pmoveState.stopLeft:
                    cameraPosFinal = new Vector3(cameraPosOr.x - fowDist, cameraPosOr.y, cameraPosOr.z);
                    // Debug.Log("CAMERA POS FINAL STOP LEFT= " + cameraPosFinal);
                    break;
                case PlayerMovement.pmoveState.wRight:
                    cameraPosFinal = new Vector3(cameraPosOr.x + runFowDist, cameraPosOr.y, cameraPosOr.z);
                    break;
                case PlayerMovement.pmoveState.wLeft:
                    cameraPosFinal = new Vector3(cameraPosOr.x - runFowDist, cameraPosOr.y, cameraPosOr.z);
                    break;
            }
            recentChange = true;
            lastPMState = PlayerMovement.instance.pmState;
            progress = 0;
            camBreakDist = Mathf.Abs(cameraPosFinal.x - PlayerT.localPosition.x) / 10;
        }
        //Debug.Log("cameraPosFinal= " + cameraPosFinal);
        if (!SpringCameraNewMode)
        {
            if (PlayerT.localPosition == cameraPosFinal)
            {
                return;
            }
            if (lastCameraPosFinal != cameraPosFinal)
            {
                actTime = 0;
            }
            if (actTime < maxTime)
            {
                actTime += Time.deltaTime;
            }
            else if (actTime > maxTime)
            {
                actTime = maxTime;
            }
            progress = actTime / maxTime;
            float x = EasingCurves.easeOutQuad(PlayerT.localPosition.x, cameraPosFinal.x, progress);
            PlayerT.localPosition = new Vector3(x, PlayerT.localPosition.y, PlayerT.localPosition.z);
        }
        else
        {
            float distToFinalPos = Mathf.Abs(cameraPosFinal.x - PlayerT.localPosition.x);
            if (distToFinalPos < 0.05)
            {
                PlayerT.localPosition = cameraPosFinal;
                camDeacc = 0;
                return;
            }
            Debug.Log("distToFinalPos" + distToFinalPos + "; camBreakDist= " + camBreakDist);
            if (distToFinalPos > camBreakDist)//Viajando a la posicion
            {
                Debug.Log("Viajando a la posicion");
                if (Mathf.Abs(playerTSpeed) >= Mathf.Abs(camMaxSpeed))//max velocidad
                {
                    Debug.Log("Max speed reached");
                    if (playerTSpeed > 0)
                        newPlayerTSpeed = camMaxSpeed;
                    else
                        newPlayerTSpeed = -camMaxSpeed;
                }
                else//acelerar si no estamos a max velocidad
                {
                    if (!recentChange)
                    {
                        Debug.Log("newPlayerTSpeed= " + newPlayerTSpeed);
                        if (cameraPosFinal.x > PlayerT.localPosition.x)
                            newPlayerTSpeed = playerTSpeed + (camAcc * Time.fixedDeltaTime);
                        else if (cameraPosFinal.x < PlayerT.localPosition.x)
                            newPlayerTSpeed = playerTSpeed - (camAcc * Time.fixedDeltaTime);
                    }
                    else
                    {
                        Debug.Log("Recent change");
                        if (cameraPosFinal.x > PlayerT.localPosition.x)
                            newPlayerTSpeed = (camAcc * Time.fixedDeltaTime);
                        else if (cameraPosFinal.x < PlayerT.localPosition.x)
                            newPlayerTSpeed = -(camAcc * Time.fixedDeltaTime);
                    }
                }
            }
            else if (distToFinalPos <= camBreakDist)//esta cerca, frenamos
            {
                Debug.Log("Frenando");
                if (camDeacc == 0)
                {
                    camDeacc = -(((Mathf.Pow(playerTSpeed, 2)) / camBreakDist) / 2);//(camBreakDist / ((-playerTSpeed * camTimeToBreak) / 2));
                    Debug.Log("camDeacc= " + camDeacc);
                }
                newPlayerTSpeed = playerTSpeed + (camDeacc * Time.deltaTime);
                Debug.Log("newPlayerTSpeed= " + newPlayerTSpeed);

            }
            PlayerT.localPosition = new Vector3(PlayerT.localPosition.x + (newPlayerTSpeed * Time.deltaTime), PlayerT.localPosition.y, PlayerT.localPosition.z);
        }
    }

    public PlayerCameraBox pCamBox;
    void focusPlayerBox()
    {
        if (instance.blockVer)
        {
            focusPosition.y = instance.minHeight;
            if (Camera.main.transform.position.y > instance.minHeight + 0.05f || Camera.main.transform.position.y < instance.minHeight - 0.05f)
            {
                Debug.Log("blockVer= true!; moving towards minHeight");
            }
        }
        focusPosition.x += pCamBox.currentLookAheadX;
    }

    public void setHotSpot(hotSpot _hotSpot)
    {
        currentHotSpot = _hotSpot;
        hsTargets = currentHotSpot.targetList;
        camMode = cameraMode.focusHotSpot;
    }

    public void stopHotSpot(hotSpot _hotSpot = null)
    {
        if (_hotSpot == null)
        {
            currentHotSpot = null;
            hsTargets = null;
            camMode = cameraMode.focusPlayerBox;
        }
    }

    void focusHotSpot()
    {
        if (currentHotSpot.useCustomSmoothTime)
        {
            finalVSmoothT = currentHotSpot.SmoothTime;
            finalHSmoothT = currentHotSpot.SmoothTime;
        }
        switch (currentHotSpot.hsMode)
        {
            case hotSpot.HotSpotMode.fixedPos:
                focusPosition.x = currentHotSpot.FixedX;
                focusPosition.y = currentHotSpot.FixedY;
                break;
            case hotSpot.HotSpotMode.fixedX:
                focusPosition.x = currentHotSpot.FixedX;
                break;
            case hotSpot.HotSpotMode.fixedY:
                focusPosition.y = currentHotSpot.FixedY;
                break;
            case hotSpot.HotSpotMode.listCentre:
                List<Vector2> pointList = new List<Vector2>();
                for (int i = 0; i < hsTargets.Count; i++)
                {
                        pointList.Add(hsTargets[i].position);
                }
                Vector2 centroid = new Vector2();
                for(int i = 0; i < pointList.Count; i++)
                {
                    centroid += pointList[i];
                }
                centroid = centroid / pointList.Count;
                focusPosition.x = centroid.x;
                focusPosition.y = centroid.y;
                break;
        }
        //Debug.Log("focusPosition= " + focusPosition+"; cameraPosition= "+transform.position);
    }
    //BLOQUEO VERTICAL Y HORIZONTAL
    public struct Ground
    {
        public GameObject ownGObject;
        public Vector2 position;
        public float bottom;

        public Ground(GameObject ownGO)
        {
            if (ownGO != null)
            {
                ownGObject = ownGO;
                position = ownGO.transform.position;
                bottom = ownGO.GetComponent<Collider2D>().bounds.center.y - ownGO.GetComponent<Collider2D>().bounds.extents.y;
            }
            else
            {
                ownGObject = null;
                position = Vector2.zero;
                bottom = 0f;
            }

        }

    }
    public cameraCollider camCol;
    bool blockHor = false;
    Collider2D playerCol;
    Ground nearestGroundUnder
    {
        set { }
        get
        {
            Ground aux = new Ground();
            if (camCol.grounds.Count == 0)
            {
                aux.ownGObject = null;
                return aux;
            }

            float distToPlayer = float.MaxValue;
            for (int i = 0; i < camCol.grounds.Count; i++)
            {
                if (camCol.grounds[i].position.y <= playerCol.bounds.center.y - playerCol.bounds.extents.y)
                {
                    float newDistToPlayer = Mathf.Abs(camCol.grounds[i].position.y - Player.position.y);
                    if (newDistToPlayer < distToPlayer)
                    {
                        distToPlayer = newDistToPlayer;
                        aux = camCol.grounds[i];
                    }
                }
            }
            return aux;
        }
    }
    [HideInInspector]
    public float minHeight;
    public bool blockVer
    {
        set { }
        get
        {
            if (nearestGroundUnder.ownGObject != null)
            {

                //Debug.Log("nearestGroundUnder= " + nearestGroundUnder.ownGObject.name + "; nearestGroundUnder.position= " + nearestGroundUnder.position);
                if (Camera.main.transform.position.y - Camera.main.orthographicSize <= nearestGroundUnder.bottom)
                {
                    if (Player.position.y >= (nearestGroundUnder.bottom + Camera.main.orthographicSize))
                    {
                        return false;
                    }
                    minHeight = nearestGroundUnder.bottom + Camera.main.orthographicSize;
                    return true;
                }
            }
                return false;
        }
    }
}
