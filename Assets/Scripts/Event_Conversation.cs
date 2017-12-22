using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event_Conversation : Event
{
    [Tooltip("Text for the conversation.")]
    public Text textoBob;
    protected int converStep;
    protected bool startConver;

    hotSpotData myHotSpot;

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

    protected override void ProceedEvent()
    {
        base.ProceedEvent();
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
    protected override void StartEvent()
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

}
