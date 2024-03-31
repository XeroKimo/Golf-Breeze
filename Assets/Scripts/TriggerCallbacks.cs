using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCallbacks : MonoBehaviour
{
    public Action<GameObject, Collider> onTriggerEntered;
    public Action<GameObject, Collider> onTriggerStay;
    public Action<GameObject, Collider> onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEntered?.Invoke(gameObject, other);
    }

    private void OnTriggerStay(Collider other)
    {
        onTriggerStay?.Invoke(gameObject, other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(gameObject, other);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
