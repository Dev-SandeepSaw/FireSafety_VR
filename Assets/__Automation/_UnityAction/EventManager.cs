using UnityEngine;
using UnityAutomation;


public class EventManager : MonoBehaviour
{
   
    public EventList[] ChooseEvents;
    [DrawButton("SetVariable")]
    public void Awake() {
        ChooseEvents.SetVariable();
    }
}
