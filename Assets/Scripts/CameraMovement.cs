using System.Collections;
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
    private Transform[] hotSpots;

    public float fowDist;
    public float runFowDist;
    //SpringCamMode1
    public float maxTime;
    private float actTime;
    private float progress;

    //SpringCamMode2
    public float camAcc;
    private float camDeacc;
    public float camMaxSpeed;
    private float camBreakDist;//distancia a la que empieza a frenar
    public float camTimeToBreak;
    private float playerTSpeed;
    private float newPlayerTSpeed;
    private float playerTLastPosX;

    private cameraMode camMode;
    private PlayerMovement.pmoveState lastPMState;
    private float speedTimer;
    public float speedFreq;

    Collider2D cameraLimits;

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
        focusHotSpot
    }

    void Start()
    {
        //Debug.Log("pmState= " + PlayerMovement.instance.pmState.ToString());
        if (PlayerMovement.instance.pmState == PlayerMovement.pmoveState.stopRight)
        {
            PlayerT.localPosition = new Vector3(cameraPosOr.x + fowDist, cameraPosOr.y, cameraPosOr.z);
        }
        else if (PlayerMovement.instance.pmState == PlayerMovement.pmoveState.stopLeft)
        {
            //Debug.Log("cameraTargetPos=" + Player.localPosition);
            PlayerT.localPosition = new Vector3(cameraPosOr.x - fowDist, cameraPosOr.y, cameraPosOr.z);
        }
        cameraPosFinal = PlayerT.localPosition;
        lastPMState = PlayerMovement.instance.pmState;
    }
    void LateUpdate()
    {
        //PlayerT.localPosition = new Vector3(PlayerT.localPosition.x, cameraPosOr.y, cameraPosOr.z);
        lastCameraPosFinal = cameraPosFinal;
        switch (camMode)
        {
            case cameraMode.focusPlayer:
                focusPlayer();
                break;
            case cameraMode.focusPlayerBox:
                PlayerCameraBox.instance.KonoUpdate();
                break;
            case cameraMode.focusHotSpot:
                focusHotSpot();
                break;
        }
        /*if (speedTimer > speedFreq)
        {
            playerTSpeed = (PlayerT.localPosition.x - playerTLastPosX) / Time.fixedDeltaTime;
            playerTLastPosX = PlayerT.localPosition.x;
            speedTimer = 0;
        }
        speedTimer += Time.deltaTime;
        camTrans.position = PlayerT.position;*/
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


    void focusHotSpot()
    {

    }

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
