using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Conversation_Bob2 : Event_Conversation {

    protected override void ProceedConversation()
    {
        switch (converStep)
        {
            case 0:
                startConver = true;
                textoBob.text = "Eso ha sido... Impresionante...";
                break;
            case 1:
                textoBob.text = "¡P-Perdón, no salgo de mi asombro!¡Gracias por salvarme la vida!";
                break;
            case 2:
                textoBob.text = "";
                startConver = false;
                break;
        }
    }

}
