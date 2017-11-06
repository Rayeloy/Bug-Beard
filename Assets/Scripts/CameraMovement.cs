using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public static CameraMovement instance;
    public EasingCurves EC;
    public Transform Player;
    public Vector3 cameraPosOr;
    private Vector3 cameraPosAct;
    private Vector3 cameraPosFinal;
    private Vector3 lastCameraPosFinal;
    private Transform[] hotSpots;

    public float fowDist;
    public float runFowDist;
    public float camAcc;
    public float camMaxSpeed;
    public float camBreakDist;//distancia a la que empieza a frenar
    public float maxTime;
    private float actTime;

    private cameraMode camMode;
    private PlayerMovement.pmoveState lastPMState;


    private void Awake()
    {
        actTime = maxTime+1;//empezar apagado
        instance = this;
        camMode = cameraMode.focusPlayer;
    }

    public enum cameraMode
    {
        focusPlayer,
        focusHotSpot
    }

	void Start ()
    {
        //Debug.Log("pmState= " + PlayerMovement.instance.pmState.ToString());
        if (PlayerMovement.instance.pmState == PlayerMovement.pmoveState.stopRight)
        {
            Player.localPosition = new Vector3(cameraPosOr.x + fowDist, cameraPosOr.y, cameraPosOr.z);
        }
        else if (PlayerMovement.instance.pmState == PlayerMovement.pmoveState.stopLeft)
        {
            //Debug.Log("cameraTargetPos=" + Player.localPosition);
            Player.localPosition = new Vector3(cameraPosOr.x - fowDist, cameraPosOr.y, cameraPosOr.z);
        }
        cameraPosFinal = Player.localPosition;
        lastPMState = PlayerMovement.instance.pmState;
    }
	
	void Update () {
        lastCameraPosFinal = cameraPosFinal;
        switch (camMode)
        {
            case cameraMode.focusPlayer:
                focusPlayer();
                break;
            case cameraMode.focusHotSpot:
                focusHotSpot();
                break;
        }
        transform.position = Player.position;
	}

    void focusPlayer()//sigue al jugador con camara muelle a la misma altura siempre
    {

        //Debug.Log("playerMovementState=" + PlayerMovement.instance.pmState.ToString());
        if (lastPMState != PlayerMovement.instance.pmState) {//cambiar camPosFinal?
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
            lastPMState = PlayerMovement.instance.pmState;
        }
        //Debug.Log("cameraPosFinal= " + cameraPosFinal);
        if (Player.localPosition == cameraPosFinal)
        {
            //Debug.Log("Reached cameraPosFinal");
            return;
        }
        if (lastCameraPosFinal != cameraPosFinal)
        {
            actTime = 0;
        }
        if (actTime < maxTime)
        {
            actTime += Time.deltaTime;
        }else if (actTime > maxTime)
        {
            actTime = maxTime;
        }
        float progress = actTime / maxTime;
        float x = EasingCurves.easeOutQuad(Player.localPosition.x,cameraPosFinal.x, progress);
        //Debug.Log("Moving camera to posx= " + x);
        Player.localPosition = new Vector3(x, Player.localPosition.y, Player.localPosition.z);
    }

    void focusHotSpot()
    {

    }
}
