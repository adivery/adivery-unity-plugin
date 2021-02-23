using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdiveryEventExecutor : MonoBehaviour
{
    public static AdiveryEventExecutor instance = null;

    private static List<Action> adEventsQueue = new List<Action>();

    private volatile static bool adEventsQueueEmpty = true;

    public static void Initialize()
    {
        if (IsActive())
        {
            return;
        }

        // Add an invisible game object to the scene
        GameObject obj = new GameObject("AdiveryMainThreadExecuter");
        obj.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(obj);
        instance = obj.AddComponent<AdiveryEventExecutor>();
    }

    public static bool IsActive()
    {
        return instance != null;
    }

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void ExecuteInUpdate(Action action)
    {
        lock (adEventsQueue)
        {
            adEventsQueue.Add(action);
            adEventsQueueEmpty = false;
        }
    }

    public static void InvokeInUpdate(UnityEvent eventParam)
    {
        ExecuteInUpdate(() =>
        {
            eventParam.Invoke();
        });
    }

    public void Update()
    {
        if (adEventsQueueEmpty)
        {
            return;
        }

        List<Action> stagedAdEventsQueue = new List<Action>();

        lock (adEventsQueue)
        {
            stagedAdEventsQueue.AddRange(adEventsQueue);
            adEventsQueue.Clear();
            adEventsQueueEmpty = true;
        }

        foreach (Action stagedEvent in stagedAdEventsQueue)
        {
            if (stagedEvent.Target != null)
            {
                stagedEvent.Invoke();
            }
        }
    }

    public void OnDisable()
    {
        instance = null;
    }
}