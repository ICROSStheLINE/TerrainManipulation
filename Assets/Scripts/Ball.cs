using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    float flashPoint = 10; // Flash Point: Temperature required to ignite using external source
    float temperature = 0;

    void Start()
    {
        
    }

    void Update()
    {
        if (temperature >= flashPoint)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Spark")
        {
            Debug.Log("Collided with Spark!");
            temperature += 10;
        }
    }
}
