using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event_Conversation : MonoBehaviour
{
    [Tooltip("Text for the conversation.")]
    public Text textoBob;
    protected bool eventFinished;
    protected bool startEvent;
    protected int converStep;
    protected bool startConver;
    [Tooltip("List of triggers that must be completed so the event can begin.")]
    public EventTrigger[] eventTriggers;
    [Tooltip("List of objectives that correspond to every EventTrigger of type objectiveCompletion.")]
    public Objective[] triggerObjectives;
    [Tooltip("Toggle if the camera moves to focus on both the player and the target.")]
    public bool doFocusTarget;
    [Tooltip("The target of the camera focus")]
    public Transform focusTarget;
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
        startConver = false;
        converStep = 0;
        textoBob.text = "";
        if (doFocusTarget)
        {
            List<Transform> targetList = new List<Transform>();
            targetList.Add(PlayerMovement.instance.gameObject.transform);
            targetList.Add(focusTarget);
            myHotSpot = new hotSpotData(hotSpotData.HotSpotMode.listCentre,targetList);
        }

        //revisión de errores
        if (eventTriggers.Length == 0)
        {
            Debug.LogError("In the event " + this + "from the object" + gameObject.name + " there are no eventTriggers. There has to be at least one.");
        }
        for (int i = 0; i < eventTriggers.Length; i++)
        {
            if(triggerObjectives.Length < numObjEvTrigger || triggerObjectives.Length > numObjEvTrigger)
            {
                Debug.LogError("In the event "+this+"from the object"+gameObject.name+" the amount of eventTriggers of type 'objectiveCompleted' does not match with the amount of Trigger Objectives");
            }  
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
        if (startConver)
        {
            //timerConversation += Time.deltaTime;
            if (Input.GetButtonDown("Jump"))
            {
                converStep++;
                ProceedConversation();
            }
        }
        if (!startConver)
        {
            giveControlBack();
        }
    }
    bool EventTriggered//si se cumplen todos los triggers
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

    bool allObjTriggered
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
    int indexTrig2Obj(int indexTrigger)
    {
        int res = -1;
        for(int i = 0; i <= indexTrigger; i++)
        {
            if (eventTriggers[i] == EventTrigger.objectiveCompleted)
            {
                res++;
            }
        }
        return res;
    }
    bool playerEntered = false;
    bool hasEnterEvTrigger
    {
        set { }
        get
        {
            bool res = false;
            for(int i = 0; i < eventTriggers.Length; i++)
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
    bool hasObjEvTrigger
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
    int numObjEvTrigger
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
    private void OnTriggerEnter2D(Collider2D col)
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

    void StartEvent()
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
            ProceedConversation();
        }
    }

    protected virtual void ProceedConversation()
    {
        switch (converStep)
        {
            case 0:
                startConver = true;
                textoBob.text = "¡Por aquí aventurero! ¡Qué alivio no encontrarme con uno de esos mounstros!";
                break;
            case 1:
                textoBob.text = "Por favor, ¡tiene que ayudarme! Es mi hermano...";
                break;
            case 2:
                textoBob.text = "¡Creo que mi hermano está en peligro!";
                break;
            case 3:
                textoBob.text = "Fue a buscar leña al bosque y creo que se adentró demasiado...";
                break;
            case 4:
                textoBob.text = "Cuando fuí a buscarle, el puente estaba roto y no puedo llegar al otro lado...";
                break;
            case 5:
                textoBob.text = "Por favor, Tiene que encontrarle... ¡He oído rugidos al otro lado!";
                break;
            case 6:
                textoBob.text = "";
                startConver = false;
                break;
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
            CameraMovement.instance.stopHotSpot();
        }
        eventFinished = true;

    }

}
