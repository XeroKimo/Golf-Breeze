using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalComponent : MonoBehaviour
{
    public bool playerCollided {  get; private set; }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerCollided = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerCollided = false;
        }
    }
}
