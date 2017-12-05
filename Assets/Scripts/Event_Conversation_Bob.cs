using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Conversation_Bob : Event_Conversation {

    protected override void ProceedConversation()
    {
        switch (converStep)
        {
            case 0:
                startConver = true;
                textoBob.text = "¡Ayuda...por favor...!";
                break;
            case 1:
                textoBob.text = "¡Aaaaagh...!";
                break;
            case 2:
                textoBob.text = "";
                startConver = false;
                break;
        }
    }

}
