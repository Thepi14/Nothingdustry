using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static WallGraphicsUpdater;

public class Destroyer : MonoBehaviour
{
    public class DestroyEvent : UnityEvent { }

    public DestroyEvent destroy;
    void Start()
    {
        if (destroy == null)
            destroy = new DestroyEvent();
        destroy.AddListener(DestroyObject);
    }
    public void DestroyObject()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
    void Update()
    {
        
    }
}
