using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour {

    protected bool eventFinished;
    protected bool startEvent;
    [Tooltip("List of triggers that must be completed so the event can begin.")]
    public EventTrigger[] eventTriggers;
    [Tooltip("List of objectives that correspond to every EventTrigger of type objectiveCompletion.")]
    public Objective[] triggerObjectives;
    public bool useTimer;
    public float maxEventTime;
    private float eventTime;
    [Tooltip("Toggle if the camera moves to focus on both the player and the target.")]
    public bool doFocusTarget;
    [Tooltip("The target of the camera focus")]
    public Transform focusTarget;
    [Tooltip("Camera shake duration. If =0, no shake.")]
    public float CameraShakeTime;
    [Tooltip("Objective that is completed when the event ends.")]
    public Objective targetObjective;
    [Tooltip("List of enemies that activate after event ends.")]
    public EnemyAI[] enemigos;

    hotSpotData myHotSpot;

    public enum EventTrigger
    {
        enter = 0,
        objectiveCompleted = 1
    }

    private void Awake()
    {
        eventFinished = false;
        startEvent = false;
        eventTime = 0;

        //revisión de errores
        if (eventTriggers.Length == 0)
        {
            Debug.LogError("In the event " + this + "from the object" + gameObject.name + " there are no eventTriggers. There has to be at least one.");
        }
        for (int i = 0; i < eventTriggers.Length; i++)
        {
            if (triggerObjectives.Length < numObjEvTrigger || triggerObjectives.Length > numObjEvTrigger)
            {
                Debug.LogError("In the event " + this + "from the object" + gameObject.name + " the amount of eventTriggers of type 'objectiveCompleted' does not match with the amount of Trigger Objectives");
            }
        }

    }
    private void Start()
    {
        if (doFocusTarget)
        {
            List<Transform> targetList = new List<Transform>();
            targetList.Add(PlayerMovement.instance.gameObject.transform);
            targetList.Add(focusTarget);
            myHotSpot = new hotSpotData(hotSpotData.HotSpotMode.listCentre, targetList);
        }
    }
    private void Update()
    {
        if (!eventFinished && !startEvent && EventTriggered)
        {
            StartEvent();
        }
        if (!eventFinished && startEvent)
        {
            ProceedEvent();
        }

    }

    protected virtual void ProceedEvent()
    {
        if (useTimer)
        {
            eventTime += Time.deltaTime;
            if (eventTime >= maxEventTime)
            {
                giveControlBack();
            }
        }
    }
    protected bool EventTriggered//si se cumplen todos los triggers
    {
        set { }
        get
        {
            bool res = true;
            for (int i = 0; i < eventTriggers.Length; i++)
            {
                switch (eventTriggers[i])
                {
                    case EventTrigger.enter:
                        if (!playerEntered) res = false;
                        break;
                    case EventTrigger.objectiveCompleted:
                        if (!triggerObjectives[indexTrig2Obj(i)].completed) res = false;
                        break;
                }
            }
            return res;
        }
    }

    protected bool allObjTriggered
    {
        set { }
        get
        {
            bool res = true;
            for (int i = 0; i < eventTriggers.Length; i++)
            {
                if (eventTriggers[i] == EventTrigger.objectiveCompleted)
                {
                    if (!triggerObjectives[indexTrig2Obj(i)].completed) res = false;
                    break;
                }
            }
            return res;
        }
    }
    protected int indexTrig2Obj(int indexTrigger)
    {
        int res = -1;
        for (int i = 0; i <= indexTrigger; i++)
        {
            if (eventTriggers[i] == EventTrigger.objectiveCompleted)
            {
                res++;
            }
        }
        return res;
    }
    protected bool playerEntered = false;
    protected bool hasEnterEvTrigger
    {
        set { }
        get
        {
            bool res = false;
            for (int i = 0; i < eventTriggers.Length; i++)
            {
                if (eventTriggers[i] == EventTrigger.enter)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }
    }
    protected bool hasObjEvTrigger
    {
        set { }
        get
        {
            bool res = false;
            for (int i = 0; i < eventTriggers.Length; i++)
            {
                if (eventTriggers[i] == EventTrigger.objectiveCompleted)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }
    }
    protected int numObjEvTrigger
    {
        set { }
        get
        {
            int res = 0;
            for (int i = 0; i < eventTriggers.Length; i++)
            {
                if (eventTriggers[i] == EventTrigger.objectiveCompleted)
                {
                    res++;
                    break;
                }
            }
            return res;
        }
    }
    protected void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("eventFinished=" + eventFinished + "; hasEnterEvTrigger=" + hasEnterEvTrigger + "; allObjTriggered=" + allObjTriggered);
        if (!eventFinished && hasEnterEvTrigger && allObjTriggered)
        {
            if (col.name == "Player")
            {
                playerEntered = true;
                StartEvent();
            }
        }
    }

    protected virtual void StartEvent()
    {
        if (!startEvent)
        {
            PlayerMovement.instance.stopPlayer = true;
            startEvent = true;
            //move camera hotSpot
            if (doFocusTarget)
            {
                CameraMovement.instance.setHotSpot(myHotSpot);
            }
            if (CameraShakeTime > 0)
            {
                CameraMovement.instance.StartShakeCamera(CameraShakeTime);
            }
        }
    }
    protected virtual void giveControlBack()
    {
        PlayerMovement.instance.stopPlayer = false;
        startEvent = false;
        for (int i = 0; i < enemigos.Length; i++)
        {
            enemigos[i].stopEnemy = false;
        }
        if (targetObjective != null)
        {
            targetObjective.Complete();
        }
        //move camera a Player
        if (doFocusTarget)
        {
            CameraMovement.instance.stopHotSpot(myHotSpot);
        }
        eventFinished = true;

    }
}
