using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public static CameraMovement instance;

    public bool SpringCameraNewMode;

    public EasingCurves EC;
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
    [Header("CAMERA FINAL VERSION")]
    private cameraMode camMode;
    public Vector2 focusPosition;
    hotSpotData currentHotSpot;
    private List<Transform> hsTargets;
    private Vector3 cameraTarget;

    float smoothVelocityX, smoothVelocityY;
    public float verticalSmoothTime, horizontalSmoothTime;
    float finalVSmoothT, finalHSmoothT;
    [HideInInspector]
    public bool LookDown = false, LookUp = false;
    [Tooltip("Distancia que se mueve la camara al mirar hacia abajo o arriba pulsando s/down o w/up.")]
    public float LookDownDist, LookUpDist;
    Vector3 cameraLastPos;

    private void Awake()
    {
        myRB = GetComponent<Rigidbody2D>();
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
        focusPlayerBoxSpeed
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
        cameraLastPos = transform.position;
        cameraTarget = Player.position;//no es correcto pero por no liarla con la focusBox
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
            case cameraMode.focusPlayerBoxSpeed:
                focusPlayerBoxSpeed();
                break;
        }
        if (LookUp)
        {
            focusPosition.y += LookUpDist;
        }
        else if (LookDown)
        {
            focusPosition.y -= LookDownDist;
        }

        //focusPosition.y = Mathf.SmoothDamp(cameraTarget.y, focusPosition.y, ref smoothVelocityY, finalVSmoothT);
        if (camMode != cameraMode.focusPlayerBoxSpeed)
        {
            //focusPosition.x = Mathf.SmoothDamp(cameraTarget.x, focusPosition.x, ref smoothVelocityX, finalHSmoothT);
            focusPosition = SuperSmoothLerp(cameraLastPos,cameraTarget,focusPosition,Time.deltaTime,17f);
            //Debug.Log("FINAL CAMERA POS= " + focusPosition);
            cameraTarget = (Vector3)focusPosition + Vector3.forward * -10;
            ShakeCamera();
            transform.position = (Vector3)focusPosition + Vector3.forward * -10;
            cameraLastPos = transform.position;

        }
        else
        {
            Vector3 aux = (Vector3)focusPosition + Vector3.forward * -10;
            if (!frenando)
                transform.position = new Vector3(transform.position.x, aux.y, aux.z);
            else
                transform.position= (Vector3)focusPosition + Vector3.forward * -10;
        }
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
        focusPosition += Vector2.right * pCamBox.currentLookAheadX;

    }
    [Header("focusPlayerBoxSpeed")]
    public float acc, maxSpeed;
    public Rigidbody2D playerRB;//Rigid Body del Camera Target en el Player
    private Rigidbody2D myRB;
    private float dir;
    bool frenando;
    float timeFrenando;
    public float MaxTimeFrenando;
    void focusPlayerBoxSpeed()
    {
        pCamBox.konoUpdate2();
        if (pCamBox.realTargetLookAhead - transform.position.x != 0)
        {
            if (playerRB.velocity.x != 0)
            {
                frenando = false;
                dir = Mathf.Sign(pCamBox.realTargetLookAhead - transform.position.x);
                if (Mathf.Abs(myRB.velocity.x) < maxSpeed)
                    myRB.velocity = new Vector2(myRB.velocity.x + ((acc * Time.deltaTime) * dir), myRB.velocity.y);
                else
                {
                    myRB.velocity = new Vector2(maxSpeed * dir, myRB.velocity.y);
                }
            }
            else//frena
            {
                if (!frenando)
                {
                    myRB.velocity = new Vector2(0, myRB.velocity.y);
                    frenando = true;
                    timeFrenando = 0;
                }
                if (timeFrenando <= MaxTimeFrenando)
                {
                    //probar a lerpear la velocidad
                    float prog = timeFrenando / MaxTimeFrenando;
                    focusPosition.x = Mathf.Lerp(transform.position.x, pCamBox.realTargetLookAhead, prog);
                    Debug.Log("FRENANDO: posActual=" + transform.position.x + "posSiguiente= " + focusPosition.x);
                    timeFrenando += Time.deltaTime;
                }
                else
                {
                    frenando = false;
                    focusPosition.x = pCamBox.realTargetLookAhead;
                }

            }
        }


    }

    public void setHotSpot(hotSpotData _hotSpotData)
    {
        currentHotSpot = _hotSpotData;
        hsTargets = currentHotSpot.targetList;
        camMode = cameraMode.focusHotSpot;
    }

    public void stopHotSpot(hotSpotData _hotSpot = null)
    {
        if (_hotSpot == null)
        {
            currentHotSpot = null;
            hsTargets = null;
            camMode = cameraMode.focusPlayerBox;
        }
    }

    bool shaking, smoothShakeStart_End;
    float TimeShaking;
    float MaxTimeShaking;
    float ShakingSize;
    float NextShakeTime;
    float MaxNextShakeTime;
    Vector2 shakedPos;
    public void shakeCameraTrial(float time)
    {
        StartShakeCamera(time,0.5f,0.08f,true);
    }
    public void StartShakeCamera(float time, float size = 0.5f,float shakeFreq=0.08f, bool _smoothShakeStart_End = false)
    {
        shaking = true;
        smoothShakeStart_End = _smoothShakeStart_End;
        shakedPos = Vector2.zero;
        TimeShaking = 0;
        MaxTimeShaking = time;
        ShakingSize = size;
        NextShakeTime = MaxNextShakeTime + 1;
    }
    void ShakeCamera()
    {
        if (shaking)
        {
            Debug.Log("SHAKING CAMERA");
            if (TimeShaking >= MaxTimeShaking)
            {
                shaking = false;
            }
            float actShakingSize = ShakingSize;
            //smoothShake
            if (smoothShakeStart_End)
            {
                if (TimeShaking < MaxTimeShaking / 5)//beggining
                {
                    float prog = TimeShaking / (MaxTimeShaking / 5);
                    actShakingSize = Mathf.Lerp(0, ShakingSize, prog);
                }
                else if (TimeShaking >= (MaxTimeShaking / 5) && TimeShaking <= (MaxTimeShaking-(MaxTimeShaking / 5)))
                {
                    actShakingSize = ShakingSize;
                }else if (TimeShaking > (MaxTimeShaking - (MaxTimeShaking / 5)))
                {
                    float timeStartEnd = (MaxTimeShaking - (MaxTimeShaking / 5));
                    float prog = (TimeShaking- timeStartEnd) / (MaxTimeShaking- timeStartEnd);
                    actShakingSize = Mathf.Lerp(ShakingSize, 0, prog);
                }
            }
            else
            {
                actShakingSize = ShakingSize;
            }

            if (NextShakeTime >= MaxNextShakeTime)
            {
                NextShakeTime = 0;
                float shakedX = Random.Range(-actShakingSize, actShakingSize);
                float shakedY = Random.Range(-actShakingSize, actShakingSize);
                shakedPos = new Vector2(shakedX, shakedY);
            }
            focusPosition = new Vector2(cameraTarget.x + shakedPos.x, cameraTarget.y + shakedPos.y);
            TimeShaking += Time.deltaTime;
            NextShakeTime += Time.deltaTime;
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
            case hotSpotData.HotSpotMode.fixedPos:
                focusPosition.x = currentHotSpot.FixedX;
                focusPosition.y = currentHotSpot.FixedY;
                break;
            case hotSpotData.HotSpotMode.fixedX:
                focusPosition.x = currentHotSpot.FixedX;
                break;
            case hotSpotData.HotSpotMode.fixedY:
                focusPosition.y = currentHotSpot.FixedY;
                break;
            case hotSpotData.HotSpotMode.listCentre:
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


    Vector3 SuperSmoothLerp(Vector3 x0, Vector3 y0, Vector3 yt, float t, float k)
    {
        Vector3 f = x0 - y0 + (yt - y0) / (k * t);
        return yt - (yt - y0) / (k * t) + f * Mathf.Exp(-k * t);
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
