using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public Action OnAnimationFinish;
    private Dictionary<string, Action> EventTable = new();

    //Debuging purposes
    public List<string> eventList = new();

    public void OnTriggerAnimationFinish()
    {
        OnAnimationFinish?.Invoke();
    }

    public void InvokeEvent(string eventName)
    {
        if (EventTable.ContainsKey(eventName))
        {
            EventTable[eventName].Invoke();
        }
        else
        {
            Debug.Log($"AnimationEventHandler's EventTable doesn't have an event with key {eventName}");
        }
    }

    public void RemoveEvent(string eventName)
    {
        EventTable.Remove(eventName);
        eventList.Remove(eventName);
    }

    public void RemoveEvent(Action eventAction)
    {
        string eventName = EventTable.First(x => x.Value == eventAction).Key;

        RemoveEvent(eventName);
    }

    public void AddEvent(string eventName, Action eventAction)
    {
        if (EventTable.ContainsKey(eventName))
        {
            Debug.Log($"AnimationEvent {eventName} already exist");
            return;
        }
        EventTable.Add(eventName, eventAction);
        eventList.Add(eventName);
    }

}